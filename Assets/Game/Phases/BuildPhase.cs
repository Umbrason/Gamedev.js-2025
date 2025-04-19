using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildPhase : IGamePhase
{
    public GameInstance Game
    {
        private get;
        set;
    }
    const string FinishedBuildPhaseSignal = "FinishedBuildPhase";
    const float BuildPhaseDurationSeconds = 30;
    private float startTime;
    private bool skipping; //should never be un-set since then this client could get stuck in this phase while the rest move on

    public readonly Dictionary<PlayerID, ResourcePledge> PledgedResources = new();

    public IEnumerator OnEnter()
    {
        HarvestResources();
        startTime = Time.unscaledTime;
        void OnUpdatePlayerIsland(NetworkMessage message)
        => Game.PlayerData[message.sender].Island = (PlayerIsland)message.content;
        Game.NetworkChannel.StartListening(UpdateIslandHeader, OnUpdatePlayerIsland);

        void OnUpdatePlayerResources(NetworkMessage message)
        => Game.PlayerData[message.sender].Resources = (Dictionary<Resource, int>)message.content;
        Game.NetworkChannel.StartListening(UpdateResourcesHeader, OnUpdatePlayerResources);
        PledgedResources[Game.ClientID] = null;
        yield return null;
    }

    IEnumerator IGamePhase.Loop()
    {
        yield return new WaitUntil(() => (Time.unscaledTime - startTime > BuildPhaseDurationSeconds) || skipping);
        skipping = true;
        yield return new WaitUntil(() => Game.NetworkChannel.WaitForAllPlayersSignal(FinishedBuildPhaseSignal, Game.ClientID));
        Game.TransitionPhase(new PledgeSummaryPhase());
    }

    const string RandomPledgeOrderDecissionHeader = "RndPledgeOrder";
    const string ShareResourcePledge = "SharePledge";
    const string EndBuildPhase = "EndBuildPhase";
    public IEnumerator OnExit()
    {
        Game.NetworkChannel.StopListening(UpdateIslandHeader);
        Game.NetworkChannel.StopListening(UpdateResourcesHeader);

        Game.NetworkChannel.BroadcastMessage(ShareResourcePledge, PledgedResources[Game.ClientID]);
        yield return new WaitUntil(() => PledgedResources.Count >= 6);

        var pledgeWithdrawalOrderPrio = new Dictionary<PlayerID, float>();
        yield return new WaitUntil(() => NetworkUtils.DistributedRandomDecision(Game.NetworkChannel, Game.ClientID, RandomPledgeOrderDecissionHeader, ref pledgeWithdrawalOrderPrio));
        var pledgeWithdrawalOrder = pledgeWithdrawalOrderPrio.OrderBy(pair => pair.Value).Select(pair => pair.Key);
        foreach (var playerID in pledgeWithdrawalOrder)
        {
            var pledges = PledgedResources[playerID].goalPledges;
            foreach (var (goalID, resources) in pledges)
            {
                goalID.GetGoal(Game).Collect(resources);
                foreach (var (resource, amount) in resources) //add back remainder
                    Game.PlayerData[playerID].Resources[resource] += amount;
            }
        }

        yield return new WaitUntil(() => Game.NetworkChannel.WaitForAllPlayersSignal(EndBuildPhase, Game.ClientID));
    }

    const string UpdateResourcesHeader = "UpdateResources";
    private void HarvestResources()
    {
        foreach (var (position, building) in Game.ClientPlayerData.Island.Buildings.OrderBy(_ => UnityEngine.Random.value + ((int)_.Value >= 7 ? 1 : 0)))
        {
            var yieldChance = building.YieldChanceAt(Game.ClientPlayerData.Island, position);
            if (UnityEngine.Random.value > yieldChance) continue;
            #region refined resources
            var opCosts = building.OperationCosts();
            if (!opCosts.Select(pair => (pair.Key, pair.Value)).All(((Resource resource, int amount) cost) => Game.ClientPlayerData.Resources.GetValueOrDefault(cost.resource) >= cost.amount))
                continue;
            foreach (var (resource, amount) in opCosts)
                Game.ClientPlayerData.Resources[resource] -= amount;
            #endregion
            var resourceYieldType = building.ResourceYieldType();
            Game.ClientPlayerData.Resources[resourceYieldType]++;
        }
        Game.NetworkChannel.BroadcastMessage(UpdateResourcesHeader, Game.ClientPlayerData.Resources);
    }

    public bool CanAffordBuilding(Building building) => Game.ClientPlayerData.HasResources(building.ConstructionCosts());
    public bool CanPlaceBuilding(HexPosition position, Building building)
        => Game.ClientPlayerData.Island.Tiles.ContainsKey(position) && //is there a tile to build on?
          !Game.ClientPlayerData.Island.Buildings.ContainsKey(position) && //is the tile free?
           CanAffordBuilding(building); //enough resources?

    const string UpdateIslandHeader = "UpdateIslandHeader";

    [PlayerAction]
    public void PlaceBuilding(HexPosition position, Building building)
    {
        if (skipping) return;
        if (!CanPlaceBuilding(position, building)) return;
        Game.ClientPlayerData.Island = Game.ClientPlayerData.Island.WithBuildings((position, building));
        Game.NetworkChannel.BroadcastMessage(UpdateIslandHeader, Game.ClientPlayerData.Island);
    }

    [PlayerAction]
    public void Skip() => skipping = true;


    [PlayerAction]
    public void PledgeResource(SharedGoalID targetGoalID, Resource resource, int amount) //negative values can "withdraw" a pledge
    {
        if (!PledgedResources.TryGetValue(Game.ClientID, out var clientPledgedResources))
            clientPledgedResources = PledgedResources[Game.ClientID] = new();
        amount = Mathf.Min(Game.ClientPlayerData.Resources[resource], amount); //cap to the max the player actually has
        if (targetGoalID.TargetRole > Game.ClientPlayerData.Role) return; //either has no role or is balanced and tried to pledge to selfish goal.

        if (!clientPledgedResources.goalPledges.ContainsKey(targetGoalID))
            clientPledgedResources.goalPledges.Add(targetGoalID, new());
        var goal = targetGoalID.GetGoal(Game);
        if (amount > 0)
        {
            var missingAmount = goal.Required.GetValueOrDefault(resource) - goal.Collected.GetValueOrDefault(resource);
            amount = Mathf.Min(missingAmount, amount);
        }
        else if (clientPledgedResources.goalPledges.ContainsKey(targetGoalID))
        {
            amount = Mathf.Max(amount, -clientPledgedResources.goalPledges[targetGoalID].GetValueOrDefault(resource));
        }
        else return;
        Game.ClientPlayerData.Resources[resource] -= amount;
        if (!clientPledgedResources.goalPledges[targetGoalID].ContainsKey(resource))
            clientPledgedResources.goalPledges[targetGoalID][resource] = amount;
        else
            clientPledgedResources.goalPledges[targetGoalID][resource] += amount;
    }

}
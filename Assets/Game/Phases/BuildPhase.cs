using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildPhase : IGamePhase, ITimedPhase
{
    public GameInstance Game
    {
        private get;
        set;
    }
    const string FinishedBuildPhaseSignal = "FinishedBuildPhase";
    public float BuildPhaseDurationSeconds = 45f;
    public float startTime { get; set; }
    private bool skipping; //should never be un-set since then this client could get stuck in this phase while the rest move on
    private PledgeSummaryPhase nextPhase;

    public event Action<HexPosition, Building, Resource, int> OnConsumeResource;
    public event Action<HexPosition, Building, Resource, int> OnHarvestResource;
    public event Action<SharedGoalID, Resource, int> OnPledgeResource;

    public readonly Dictionary<PlayerID, ResourcePledge> PledgedResources = new();

    public float TimeRemaining => startTime - Time.unscaledTime + BuildPhaseDurationSeconds;
    public float Duration => BuildPhaseDurationSeconds;


    public IEnumerator OnEnter()
    {
        Game.Turn++;
        Debug.Log("Turn "+Game.Turn);

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
        yield return new WaitUntil(() => Game.NetworkChannel.WaitForAllPlayersSignal(FinishedBuildPhaseSignal));

        nextPhase = new PledgeSummaryPhase();
        Game.TransitionPhase(nextPhase);
    }

    const string RandomPledgeOrderDecissionHeader = "RndPledgeOrder";
    const string ShareResourcePledge = "SharePledge";
    const string EndBuildPhase = "EndBuildPhase";
    public IEnumerator OnExit()
    {
        Game.NetworkChannel.BroadcastMessage(UpdateResourcesHeader, Game.ClientPlayerData.Resources);
        Game.NetworkChannel.BroadcastMessage(ShareResourcePledge, PledgedResources[Game.ClientID] ?? new());
        void OnPledgeResources(NetworkMessage message) { PledgedResources[message.sender] = (ResourcePledge)message.content; }
        Game.NetworkChannel.StartListening(ShareResourcePledge, OnPledgeResources);
        yield return new WaitUntil(() => PledgedResources.Count >= NetworkUtils.playerCount);
        Game.NetworkChannel.StopListening(UpdateIslandHeader);
        Game.NetworkChannel.StopListening(UpdateResourcesHeader);
        Game.NetworkChannel.StopListening(ShareResourcePledge);

        var pledgeWithdrawalOrderPrio = (Dictionary<PlayerID, float>)null;
        yield return new WaitUntil(() => NetworkUtils.DistributedRandomDecision(Game.NetworkChannel, RandomPledgeOrderDecissionHeader, ref pledgeWithdrawalOrderPrio));
        var pledgeWithdrawalOrder = pledgeWithdrawalOrderPrio.OrderBy(pair => pair.Value).Select(pair => pair.Key);

        Dictionary<PlayerID, Dictionary<Resource, int>> Receipts = new();
        foreach (var playerID in pledgeWithdrawalOrder)
        {
            if (PledgedResources[playerID] == null) continue;
            var pledges = PledgedResources[playerID].goalPledges;
            foreach (var (goalID, resources) in pledges)
            {
                var resourcesCopy = resources.Copy();
                if (!Receipts.ContainsKey(playerID)) Receipts[playerID] = new();

                var receipt = goalID.TargetRole == PlayerRole.Balanced ? Receipts[playerID] : null;

                goalID.GetGoal(Game).Collect(resourcesCopy, receipt);
                foreach (var (resource, amount) in resourcesCopy.ToArray()) //add back remainder
                    Game.PlayerData[playerID][resource] += amount;
            }
        }

        nextPhase.OfferedResources = Receipts;

        yield return new WaitUntil(() => Game.NetworkChannel.WaitForAllPlayersSignal(EndBuildPhase));
    }
    public const int SecretTaskRewardResourceMultiplier = 2;
    const string UpdateResourcesHeader = "UpdateResources";
    private void HarvestResources()
    {
        var multiplier = Game.ClientPlayerData.SecretGoal.Evaluate(Game.ClientPlayerData) ? SecretTaskRewardResourceMultiplier : 1;
        foreach (var (position, building) in Game.ClientPlayerData.Island.Buildings.OrderBy(_ => UnityEngine.Random.value + ((int)_.Value >= 7 ? 1 : 0)))
        {
            var expectedYield = building.ExpectedYield(Game.ClientPlayerData.Island, position);
            var actualYield = Mathf.FloorToInt(expectedYield) + (UnityEngine.Random.value <= (expectedYield % 1f) ? 1 : 0);
            #region refined resources
            var opCosts = building.OperationCosts();
            if (!opCosts.Select(pair => (pair.Key, pair.Value)).All(((Resource resource, int amount) cost) => Game.ClientPlayerData[cost.resource] >= cost.amount))
                continue;
            foreach (var (resource, amount) in opCosts)
            {
                OnConsumeResource?.Invoke(position, building, resource, amount);
                Game.ClientPlayerData[resource] -= amount;
            }
            #endregion
            var resourceYieldType = building.ResourceYieldType();
            Game.ClientPlayerData[resourceYieldType] += actualYield;
            OnHarvestResource?.Invoke(position, building, resourceYieldType, actualYield);
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
        foreach (var (resource, cost) in building.ConstructionCosts())
        {
            Game.ClientPlayerData[resource] -= cost;
            OnConsumeResource?.Invoke(position, building, resource, cost);
        }
        Game.ClientPlayerData.Island = Game.ClientPlayerData.Island.WithBuildings((position, building));
        Game.NetworkChannel.BroadcastMessage(UpdateIslandHeader, Game.ClientPlayerData.Island);
        Game.NetworkChannel.BroadcastMessage(UpdateResourcesHeader, Game.ClientPlayerData.Resources);

        SoundAndMusicController.Instance.PlaySFX(SFXType._33_DropBuildingOnMap, Game.ClientID);
    }

    [PlayerAction]
    public void Skip() => skipping = true;


    [PlayerAction]
    public void PledgeResource(SharedGoalID targetGoalID, Resource resource, int amount) //negative values can "withdraw" a pledge
    {
        if (!PledgedResources.TryGetValue(Game.ClientID, out var clientPledgedResources))
            clientPledgedResources = PledgedResources[Game.ClientID] = new();
        clientPledgedResources ??= PledgedResources[Game.ClientID] = new();//if key exists but value is null

        amount = Mathf.Min(Game.ClientPlayerData[resource], amount); //cap to the max the player actually has
        if (targetGoalID.TargetRole > Game.ClientPlayerData.Role) return; //either has no role or is balanced and tried to pledge to selfish goal.

        if (!clientPledgedResources.goalPledges.ContainsKey(targetGoalID))
            ((Dictionary<SharedGoalID, IReadOnlyDictionary<Resource, int>>)clientPledgedResources.goalPledges).Add(targetGoalID, new Dictionary<Resource, int>());
        var goal = targetGoalID.GetGoal(Game);
        if (amount > 0)
        {
            var missingAmount = goal.Required.GetValueOrDefault(resource) - goal.Collected.GetValueOrDefault(resource) - clientPledgedResources.goalPledges[targetGoalID].GetValueOrDefault(resource);
            amount = Mathf.Min(missingAmount, amount);
        }
        else if (clientPledgedResources.goalPledges.ContainsKey(targetGoalID))
        {
            amount = Mathf.Max(amount, -clientPledgedResources.goalPledges[targetGoalID].GetValueOrDefault(resource));
        }
        else return; //doesnt have any resources pledged but tried to withdraw some
        if (amount == 0) return;
        OnPledgeResource?.Invoke(targetGoalID, resource, amount);
        Game.ClientPlayerData[resource] -= amount;
        if (!clientPledgedResources.goalPledges[targetGoalID].ContainsKey(resource))
            ((Dictionary<Resource, int>)clientPledgedResources.goalPledges[targetGoalID])[resource] = amount;
        else
            ((Dictionary<Resource, int>)clientPledgedResources.goalPledges[targetGoalID])[resource] += amount;
        Game.NetworkChannel.BroadcastMessage(UpdateResourcesHeader, Game.ClientPlayerData.Resources);
        Game.NetworkChannel.BroadcastMessage(ShareResourcePledge, PledgedResources[Game.ClientID]);
    }

    public bool CanSkip() => true;
}
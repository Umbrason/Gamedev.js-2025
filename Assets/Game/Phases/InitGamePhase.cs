using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InitGamePhase : IGamePhase
{
    const string RandomGoalHeader = "RndGoal";
    const string RandomEvilGoalHeader = "RndEvilGoal";
    const string RandomRoleIndexHeader = "RndRoleIdx";
    const string RandomFactionIndexHeader = "RndFactIdx";
    const string RandomSecretGoalIndexHeader = "ScrtGoalIdx";
    const string ShareIslandState = "ShareIslandState";

    const int BalanceFactionSubgoalCount = 4;
    const int SelfishFactionSubgoalCount = 2;
    public GameInstance Game { private get; set; }

    public IEnumerator OnEnter()
    {
        #region Start Randomness Requests
        var RandomRoleIndexResults = (Dictionary<PlayerID, float>)null;
        var RandomFactionIndexResults = (Dictionary<PlayerID, float>)null;
        var RandomSecretGoalIndexResults = (Dictionary<PlayerID, float>)null;


        yield return new WaitUntil(() =>
            Game.NetworkChannel.DistributedRandomDecision(Game.ClientID, RandomRoleIndexHeader, ref RandomRoleIndexResults) &&
            Game.NetworkChannel.DistributedRandomDecision(Game.ClientID, RandomFactionIndexHeader, ref RandomFactionIndexResults) &&
            Game.NetworkChannel.DistributedRandomDecision(Game.ClientID, RandomSecretGoalIndexHeader, ref RandomSecretGoalIndexResults)
        );
        #endregion

        #region Decide BalanceFaction Goals
        var BalanceFactionGoals = new List<SharedGoal>();
        for (int i = 0; i < BalanceFactionSubgoalCount; i++)
        {
            var RandomGoalResults = (Dictionary<PlayerID, float>)null;
            yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(Game.ClientID, RandomGoalHeader, ref RandomGoalResults));
            //Game.NetworkChannel.DistributedRandomDecision(Game.ClientID, RandomGoalHeader, ref RandomGoalResults);
            foreach(var prn in RandomGoalResults)
            {
                Debug.Log($"{prn.Key} : {prn.Value}");
            }
            var SharedGoalIndex = Mathf.FloorToInt(GoalTemplates.BalanceFaction.Count * RandomGoalResults.Values.Sum() / 6f);
            BalanceFactionGoals.Add(GoalTemplates.BalanceFaction[SharedGoalIndex]);
        }
        Game.BalancedFactionGoals = BalanceFactionGoals;
        #endregion

        #region Decide SelfishFaction Goals
        var SelfishFactionGoals = new List<SharedGoal>();
        for (int i = 0; i < SelfishFactionSubgoalCount; i++)
        {
            var RandomEvilGoalResults = (Dictionary<PlayerID, float>)null;
            yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(Game.ClientID, RandomEvilGoalHeader, ref RandomEvilGoalResults));
            //Game.NetworkChannel.DistributedRandomDecision(Game.ClientID, RandomEvilGoalHeader, ref RandomEvilGoalResults);
            var EvilGoalIndex = Mathf.FloorToInt(GoalTemplates.SelfishFaction.Count * RandomEvilGoalResults.Values.Sum() / 6);
            SelfishFactionGoals.Add(GoalTemplates.SelfishFaction[EvilGoalIndex]);
        }
        Game.SelfishFactionGoals = SelfishFactionGoals;
        #endregion

        #region Roles
        var playerIDsByRolesIndex = RandomRoleIndexResults.OrderBy(pair => pair.Value).Select(pair => pair.Key);
        Game.PlayerData = new Dictionary<PlayerID, PlayerData>();
        for (int i = 0; i < 6; i++) Game.PlayerData[(PlayerID)i] = new();

        foreach (var player in playerIDsByRolesIndex.Take(2)) Game.PlayerData[player].Role = PlayerRole.Selfish;
        foreach (var player in playerIDsByRolesIndex.Skip(2)) Game.PlayerData[player].Role = PlayerRole.Balanced;
        #endregion

        #region Factions
        var faction = 0;
        var playerIDsByFactionIndex = RandomFactionIndexResults.OrderBy(pair => pair.Value).Select(pair => pair.Key);
        foreach (var player in playerIDsByFactionIndex) Game.PlayerData[player].Faction = (PlayerFactions)(++faction);
        #endregion

        #region Secret Individual Goal
        foreach (var player in RandomSecretGoalIndexResults.Keys)
            Game.PlayerData[player].SecretGoal = GoalTemplates.IndividualSecretTasks[Mathf.FloorToInt(RandomSecretGoalIndexResults[player] * GoalTemplates.IndividualSecretTasks.Count)];
        #endregion

        #region Player Island
        void OnIslandUpdateRecieved(NetworkMessage message) { Game.PlayerData[message.sender].Island = (PlayerIsland)message.content; }
        Game.NetworkChannel.StartListening(ShareIslandState, OnIslandUpdateRecieved);
        Game.PlayerData[Game.ClientID].Island = Game.MapGenerator.Generate();
        Game.NetworkChannel.BroadcastMessage(ShareIslandState, Game.PlayerData[Game.ClientID].Island);
        #endregion

        yield return new WaitUntil(() => Game.PlayerData.Values.All(data => data.Island != PlayerIsland.Empty));
        //Game.PlayerData.Values.All(data => data.Island != PlayerIsland.Empty);
        Game.TransitionPhase(new BuildPhase());

        //yield return new();
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }
}
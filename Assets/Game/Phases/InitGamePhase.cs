using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LobbyManager;

public class InitGamePhase : IGamePhase
{
    const string RandomGoalHeader = "RndGoal";
    const string RandomEvilGoalHeader = "RndEvilGoal";
    const string RandomRoleIndexHeader = "RndRoleIdx";
    const string RandomFactionIndexHeader = "RndFactIdx";
    const string RandomSecretGoalIndexHeader = "ScrtGoalIdx";
    const string ShareIslandState = "ShareIslandState";

    const int BalanceFactionSubgoalCount = 1;
    const int SelfishFactionSubgoalCount = 1;
    public GameInstance Game { private get; set; }

    public IEnumerator OnEnter()
    {
        #region Start Randomness Requests (parallel)
        var RandomRoleIndexResults = (Dictionary<PlayerID, float>)null;
        var RandomFactionIndexResults = (Dictionary<PlayerID, float>)null;
        var RandomSecretGoalIndexResults = (Dictionary<PlayerID, float>)null;

        bool roleDone = false, factionDone = false, secretGoalDone = false;

        IEnumerator WaitForRandomRoleIndex()
        {
            yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(RandomRoleIndexHeader, ref RandomRoleIndexResults));
            roleDone = true;
        }

        IEnumerator WaitForRandomFactionIndex()
        {
            yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(RandomFactionIndexHeader, ref RandomFactionIndexResults));
            factionDone = true;
        }

        IEnumerator WaitForRandomSecretGoalIndex()
        {
            yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(RandomSecretGoalIndexHeader, ref RandomSecretGoalIndexResults));
            secretGoalDone = true;
        }

        GameNetworkManager.Instance.RunCoroutine(WaitForRandomRoleIndex());
        GameNetworkManager.Instance.RunCoroutine(WaitForRandomFactionIndex());
        GameNetworkManager.Instance.RunCoroutine(WaitForRandomSecretGoalIndex());

        yield return new WaitUntil(() => roleDone && factionDone && secretGoalDone);
        #endregion

        #region Decide BalanceFaction Goals
        var BalanceFactionGoals = new List<SharedGoal>();
        HashSet<int> pickedBalancedGoals = new();
        for (int i = 0; i < BalanceFactionSubgoalCount; i++)
        {
            var RandomGoalResults = (Dictionary<PlayerID, float>)null;
            yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(RandomGoalHeader, ref RandomGoalResults));
            var SharedGoalIndex = Mathf.FloorToInt(GameSettings.BalanceGoals.Count * (RandomGoalResults.Values.Sum() % 1f));
            for (int j = 0; j < GameSettings.BalanceGoals.Count; j++)
            {
                if (!pickedBalancedGoals.Contains(SharedGoalIndex)) break;
                SharedGoalIndex++;
                SharedGoalIndex %= GameSettings.BalanceGoals.Count;
            }
            pickedBalancedGoals.Add(SharedGoalIndex);
            BalanceFactionGoals.Add(GameSettings.BalanceGoals[SharedGoalIndex]);
        }
        Game.BalancedFactionGoals = BalanceFactionGoals;
        #endregion

        #region Decide SelfishFaction Goals
        var SelfishFactionGoals = new List<SharedGoal>();
        HashSet<int> pickedSelfishGoals = new();
        for (int i = 0; i < SelfishFactionSubgoalCount; i++)
        {
            var RandomEvilGoalResults = (Dictionary<PlayerID, float>)null;
            yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(RandomEvilGoalHeader, ref RandomEvilGoalResults));
            //Game.NetworkChannel.DistributedRandomDecision( RandomEvilGoalHeader, ref RandomEvilGoalResults);
            var EvilGoalIndex = Mathf.FloorToInt(GameSettings.SelfishGoals.Count * (RandomEvilGoalResults.Values.Sum() % 1f));
            for (int j = 0; j < GameSettings.SelfishGoals.Count; j++)
            {
                if (!pickedBalancedGoals.Contains(EvilGoalIndex)) break;
                EvilGoalIndex++;
                EvilGoalIndex %= GameSettings.SelfishGoals.Count;
            }
            pickedSelfishGoals.Add(EvilGoalIndex);
            SelfishFactionGoals.Add(GameSettings.SelfishGoals[EvilGoalIndex]);
        }
        Game.SelfishFactionGoals = SelfishFactionGoals;
        #endregion

        #region Roles
        var playerIDsByRolesIndex = RandomRoleIndexResults.OrderBy(pair => pair.Value).Select(pair => pair.Key);
        Game.PlayerData = new Dictionary<PlayerID, PlayerData>();
        for (int i = 0; i < NetworkUtils.playerCount; i++)
        {
            Game.PlayerData[(PlayerID)i] = new();

            Player player = GameNetworkManager.Instance.Players.FirstOrDefault(p => p.player_gameID == i);
            if (player != null)
            {
                Game.PlayerData[(PlayerID)i].Nickname = player.player_name;
            }
        }

        foreach (var player in playerIDsByRolesIndex.Take(2)) Game.PlayerData[player].Role = PlayerRole.Selfish;
        foreach (var player in playerIDsByRolesIndex.Skip(2)) Game.PlayerData[player].Role = PlayerRole.Balanced;
        #endregion

        #region Factions
        var faction = 0;
        var playerIDsByFactionIndex = RandomFactionIndexResults.OrderBy(pair => pair.Value).Select(pair => pair.Key);
        foreach (var player in playerIDsByFactionIndex) Game.PlayerData[player].Faction = (PlayerFaction)(++faction);
        var clientFaction = Game.ClientPlayerData.Faction;
        #endregion

        #region Secret Individual Goal
        foreach (var player in RandomSecretGoalIndexResults.Keys)
            Game.PlayerData[player].SecretGoal = GoalTemplates.IndividualSecretTasks[Mathf.FloorToInt(RandomSecretGoalIndexResults[player] * GoalTemplates.IndividualSecretTasks.Count)];
        #endregion

        #region Player Island
        FactionData factionData = null;
        foreach (FactionData data in GameSettings.Factions)
        {
            if (data.Faction != clientFaction) continue;
            factionData = data;
            break;
        }
        if (factionData == null) Debug.LogError(clientFaction + "not found in GameInstance.Factions");

        void OnIslandUpdateRecieved(NetworkMessage message) { Game.PlayerData[message.sender].Island = (PlayerIsland)message.content; }
        Game.NetworkChannel.StartListening(ShareIslandState, OnIslandUpdateRecieved);

        Game.PlayerData[Game.ClientID].Island = factionData.GenerateIsland(size: 5);
        Game.PlayerData[Game.ClientID].Resources = factionData.StartingResouces;

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
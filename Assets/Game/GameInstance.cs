using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public PlayerID ClientID => NetworkChannel?.PlayerID ?? PlayerID.None;
    public INetworkChannel NetworkChannel { get; set; }
    public Dictionary<PlayerID, PlayerData> PlayerData { get; set; }
    public PlayerData ClientPlayerData { get => PlayerData?.GetValueOrDefault(ClientID); }

    public IReadOnlyList<SharedGoal> BalancedFactionGoals { get; set; }
    public IReadOnlyList<SharedGoal> SelfishFactionGoals { get; set; }
    /* public event Action<IGamePhase> OnGoalsProgressed; */

    public IGamePhase CurrentPhase { get; private set; }
    public event Action<IGamePhase> OnPhaseChanged;

    public int Turn {  get; set; } = 0;

    private IGamePhase RequestedTransition = new InitGamePhase();//new LobbyPhase();

    public void Start()
    {
        if (GameNetworkManager.Instance.availableChannels.Count >= 1) NetworkChannel = GameNetworkManager.Instance.availableChannels.Dequeue();
        else Debug.LogError("Network manager has no more network channels to distribute. Try starting the game via the lobby?");
        StartCoroutine(Loop());
    }

    public IEnumerator Loop()
    {
        var currentLoop = (IEnumerator)null;
        while (true)
        {
            if (RequestedTransition != null)
            {
                OnPhaseChanged?.Invoke(null);
                yield return CurrentPhase?.OnExit();
                RequestedTransition.Game = this;
                CurrentPhase = RequestedTransition;
                currentLoop = CurrentPhase.Loop();
                OnPhaseChanged?.Invoke(RequestedTransition);
                RequestedTransition = null;
                yield return CurrentPhase?.OnEnter();
            }
            yield return currentLoop.Current;
            if (!currentLoop?.MoveNext() ?? false)
                currentLoop = null;
        }
    }

    public void TransitionPhase(IGamePhase newPhase)
    {
        RequestedTransition = newPhase;
    }

    [ContextMenu("Debug PlayerData")]
    private void DebugPlayerData()
    {
        PlayerData p = ClientPlayerData;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("PlayerData: ");
        sb.Append("Nickname: ");
        sb.AppendLine(p.Nickname);
        sb.Append("Faction: ");
        sb.AppendLine(p.Faction.ToString());
        sb.Append("Role: ");
        sb.AppendLine(p.Role.ToString());
        //stringBuilder.AppendLine(p.SecretGoal.ToString());

        Debug.Log(sb.ToString());
    }
}
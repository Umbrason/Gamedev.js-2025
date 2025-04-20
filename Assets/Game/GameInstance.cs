

using System;
using System.Collections;
using System.Collections.Generic;
using MapGenerator;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public PlayerID ClientID => NetworkChannel.PlayerID;
    [field: SerializeField] public TilesBoardGeneratorData MapGenerator { get; private set; }
    public INetworkChannel NetworkChannel { get; set; }
    public Dictionary<PlayerID, PlayerData> PlayerData { get; set; }
    public PlayerData ClientPlayerData { get => PlayerData[ClientID]; set => PlayerData[ClientID] = value; }
    public IReadOnlyList<SharedGoal> BalancedFactionGoals { get; set; }
    public IReadOnlyList<SharedGoal> SelfishFactionGoals { get; set; }
    public IGamePhase CurrentPhase { get; private set; }
    public event Action<IGamePhase> OnPhaseChanged;

    private IGamePhase RequestedTransition = new InitGamePhase();//new LobbyPhase();

    public void Start()
    {
        NetworkChannel = GameNetworkManager.Instance.availableChannels.Dequeue();
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
}
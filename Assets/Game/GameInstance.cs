

using System;
using System.Collections;
using System.Collections.Generic;
using MapGenerator;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public PlayerID ClientID => NetworkChannel.PlayerID;
    [field: SerializeField] public TilesBoardGeneratorData MapGenerator { get; private set;}
    public INetworkChannel NetworkChannel { get; set; } = new LocalDummyNetwork();
    public Dictionary<PlayerID, PlayerData> PlayerData { get; set; }
    public PlayerData ClientPlayerData { get => PlayerData[ClientID]; set => PlayerData[ClientID] = value; }
    public IReadOnlyList<SharedGoal> BalancedFactionGoals { get; set; }
    public IReadOnlyList<SharedGoal> SelfishFactionGoals { get; set; }
    public IGamePhase CurrentPhase { get; private set; }
    public event Action<IGamePhase> OnPhaseChanged;

    private IGamePhase RequestedTransition = new InitGamePhase();//new LobbyPhase();

    public void Start()
    {
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
                yield return CurrentPhase?.OnEnter();
                OnPhaseChanged?.Invoke(RequestedTransition);
                currentLoop = CurrentPhase.Loop();
                RequestedTransition = null;
            }
            if (!currentLoop?.MoveNext() ?? false)
                currentLoop = null;
            yield return null;
        }
    }

    public void TransitionPhase(IGamePhase newPhase)
    {
        RequestedTransition = newPhase;
    }
}
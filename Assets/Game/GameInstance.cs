

using System.Collections;
using System.Collections.Generic;

public class GameInstance
{
    public PlayerID ClientID { get; set; }
    public INetworkChannel NetworkChannel { get; set; }
    public Dictionary<PlayerID, PlayerData> PlayerData { get; set; }
    public PlayerData ClientPlayerData { get => PlayerData[ClientID]; set => PlayerData[ClientID] = value; }
    public IReadOnlyList<SharedGoal> BalancedFactionGoals  { get; set; }
    public IReadOnlyList<SharedGoal> SelfishFactionGoals { get; set; }
    private IGamePhase CurrentPhase;
    public IEnumerator Start()
    {
        yield return TransitionPhase(new LobbyPhase());
    }

    public IEnumerator TransitionPhase(IGamePhase newPhase)
    {
        yield return CurrentPhase?.OnExit();
        newPhase.Game = this;
        CurrentPhase = newPhase;
        yield return CurrentPhase?.OnEnter();
    }
}
using System.Collections.Generic;

public static class GameState
{
    public static Dictionary<PlayerID, PlayerData> PlayerData { get; set; }
    public static List<Goal> SharedGoal { get; set; }
    public static List<Goal> EvilGoal { get; set; }
}

//Create Lobby
//Start Immediately -> 5 bots

//Players Join
//Start at any time
//Rest is filled with bots
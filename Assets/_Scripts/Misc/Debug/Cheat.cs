using UnityEngine;
using System;
using System.Collections.Generic;

public class Cheat : MonoBehaviour
{
    public GameInstance GameInstance;

    public void AddResources(int quantity)
    {
        foreach (Resource res in Enum.GetValues(typeof(Resource)))
        {
            if (res == Resource.None) continue;
            GameInstance.ClientPlayerData[res] = GameInstance.ClientPlayerData[res] + quantity;
        }

        GameInstance.NetworkChannel.BroadcastMessage("UpdateResources", GameInstance.ClientPlayerData.Resources);
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

public class Cheat : MonoBehaviour
{
    public GameInstance GameInstance;

    public void AddResources(int quantity)
    {
        GameInstance.ClientPlayerData.Resources ??= new();


        foreach (Resource res in Enum.GetValues(typeof(Resource)))
        {
            if (res == Resource.None) continue;


            GameInstance.ClientPlayerData.Resources[res] = GameInstance.ClientPlayerData.Resources.GetValueOrDefault(res) + quantity;
        }

        GameInstance.NetworkChannel.BroadcastMessage("UpdateResources", GameInstance.ClientPlayerData.Resources);
    }
}

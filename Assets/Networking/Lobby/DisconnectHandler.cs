using System;
using UnityEngine;
using TMPro;

public class DisconnectHandler : Singleton<DisconnectHandler>
{
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private TMP_Text outputText;

	public void Disconnect()
	{

	}

    public void Leave()
    {

    }

    public void RoomClosed()
    {

    }
}
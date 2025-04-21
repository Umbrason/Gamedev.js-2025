using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using static LobbyManager;
using static UnityEngine.Rendering.DebugUI;

public class PledgeSummaryHandler : GamePhaseHandler<PledgeSummaryPhase>
{
    [SerializeField] OfferingView[] offeredWiews;
    [SerializeField] GameObject parent;

    public override void OnPhaseEntered()
    {
        StartCoroutine(ShowOfferings());
    }

    IEnumerator ShowOfferings()
    {
        parent.SetActive(true);
        for (int i = 0; i < offeredWiews.Length; i++)
        {
            PlayerID id = (PlayerID)i;
            offeredWiews[i].OfferedResources = null;//hides everything
        }

        for (int i = 0; i < offeredWiews.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);
            PlayerID id = (PlayerID)i;

            Dictionary<Resource, int> R = new();
            offeredWiews[i].OfferedResources = Phase.OfferedResources.GetValueOrDefault(id);
        }
    }

    public override void OnPhaseExited()
    {
        StopAllCoroutines();
        parent.SetActive(false);
    }
}

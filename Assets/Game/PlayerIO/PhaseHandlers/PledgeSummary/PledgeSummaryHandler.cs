using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class PledgeSummaryHandler : GamePhaseHandler<PledgeSummaryPhase>
{
    [SerializeField] OfferingView[] offeredWiews;
    [SerializeField] GameObject parent;

    [Header("Ordered by resource id (Compost, Ink, Mushrooms, Firebugs, Wisps, ManaStones)")]
    [SerializeField] private BalancedGoalProgressDisplay[] ProgressImages;
    private readonly Dictionary<Resource, BalancedGoalProgressDisplay> progressImagesByResource = new();

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < 6; i++)
        {
            var resource = (Resource)(7 + i);
            progressImagesByResource[resource] = ProgressImages[i];
            ProgressImages[i].NormalizedProgress = 0;
        }
    }

    public override void OnPhaseEntered()
    {
        parent.SetActive(true);
        StartCoroutine(ShowOfferings());
        foreach (var resource in Phase.OfferedResources.Values.SelectMany(dict => dict?.Where(pair => pair.Value > 0).Select(pair => pair.Key)).Distinct())
            StartCoroutine(LerpProgress(resource));
    }

    float animationDuration = 1f;
    IEnumerator LerpProgress(Resource resource)
    {
        if (!progressImagesByResource.ContainsKey(resource))
        {
            Debug.LogError($"Missing resource progress image for {resource}");
            yield break;
        }
        var offeredAmount = Phase.OfferedResources.Values.Select((dict) => dict.GetValueOrDefault(resource)).Sum();
        var newValue = Game.BalancedFactionGoals.Sum(goal => goal.Collected.GetValueOrDefault(resource));
        var oldValue = newValue - offeredAmount;
        var required = Game.BalancedFactionGoals.Sum(goal => goal.Required.GetValueOrDefault(resource));

        var progressBefore = oldValue / required;
        var progressAfter = newValue = required;
        var t = 0f;
        while (t < 1)
        {
            t += Time.unscaledTime / animationDuration;
            progressImagesByResource[resource].NormalizedProgress = Mathf.Lerp(progressBefore, progressAfter, t);
            yield return null;
        }
        progressImagesByResource[resource].NormalizedProgress = progressAfter;
        yield return null;
    }

    IEnumerator ShowOfferings()
    {
        for (int i = 0; i < offeredWiews.Length; i++)
        {
            PlayerID id = (PlayerID)i;
            offeredWiews[i].OfferedResources = null;//hides everything
            offeredWiews[i].Faction = Game.PlayerData[id].Faction;
            offeredWiews[i].Nickname = Game.PlayerData[id].Nickname ?? Game.PlayerData[id].Faction.ToString();
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

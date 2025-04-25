using System.Collections;
using UnityEngine;

public class InitGamePhaseHandler : GamePhaseHandler<InitGamePhase>
{
    [SerializeField] CanvasGroup FadeInCanvasGroup;
    [SerializeField] float fadeInDuration = 1f;

    IEnumerator FadeInRoutine()
    {
        var t = 0f;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / fadeInDuration;
            FadeInCanvasGroup.alpha = 1 - t * t;
            yield return null;
        }
        FadeInCanvasGroup.gameObject.SetActive(false);
    }
    public override void OnPhaseExited()
    {
        StartCoroutine(FadeInRoutine());
    }
}

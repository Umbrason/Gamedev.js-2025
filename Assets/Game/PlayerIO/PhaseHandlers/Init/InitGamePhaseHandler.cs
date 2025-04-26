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
    public override void OnPhaseEntered()
    {
        FadeInCanvasGroup.gameObject.SetActive(true);

    }
    public override void OnPhaseExited()
    {
        StartCoroutine(FadeInRoutine());

        switch (Game.PlayerData[Game.ClientID].Role)
        {
            case PlayerRole.Balanced:
                SoundAndMusicController.Instance.PlayMusic(MusicType.soundtrackGoodLoopable, Game.ClientID);
                SoundAndMusicController.Instance.PlayAmbience(MusicType.ambienceGoodLoopable, Game.ClientID); break;
            case PlayerRole.Selfish:
                SoundAndMusicController.Instance.PlayMusic(MusicType.soundtrackSusLoopable, Game.ClientID);
                SoundAndMusicController.Instance.PlayAmbience(MusicType.ambienceSusLoopable, Game.ClientID); break;
        }
    }
}

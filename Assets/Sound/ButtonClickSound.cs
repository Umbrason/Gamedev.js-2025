using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    public enum SFXType
    {
        GeneralButtonClick,
        ConfirmButton,
        VoteSusButton,
        PauseButton,
        MainMenuNavButtons
    }

    [SerializeField]
    private SFXType soundEffect = SFXType.GeneralButtonClick;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        var sfxCollection = SoundAndMusicController.Instance.SfxClips;
        AudioClip clip = GetClipFromEnum(soundEffect, sfxCollection);
        SoundAndMusicController.Instance.PlaySFX(clip);
    }

    private AudioClip GetClipFromEnum(SFXType type, SFXClipCollection sfx)
    {
        return type switch
        {
            SFXType.GeneralButtonClick => sfx.generalButtonClick,
            SFXType.ConfirmButton => sfx.confirmButton,
            SFXType.VoteSusButton => sfx.voteSusButton,
            SFXType.PauseButton => sfx.pauseButton,
            SFXType.MainMenuNavButtons => sfx.mainMenuNavButtons,
            _ => null,
        };
    }
}
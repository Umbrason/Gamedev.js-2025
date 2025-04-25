using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum SFXType
    {
        GeneralButtonClick,
        PopupOpens,
        BumbiJoins,
        BumbiLeaves,
        LykiJoins,
        LykiLeaves,
        PigynJoins,
        PigynLeaves,
        GumbiJoins,
        GumbiLeaves,
        SeltasJoins,
        SeltasLeaves,
        PomPomJoins,
        PomPomLeaves,
        GoodSideWins,
        SusSideWins,
        RoundStart,
        RoundEnd,
        BuildVotingTimeStart,
        BuildVotingTimeEnd,
        SusVotingTimeStart,
        SusVotingTimeEnd,
        TimerTicking,
        LookAtBumbiIsland,
        LookAtLykiIsland,
        LookAtPigynIsland,
        LookAtGumbiIsland,
        LookAtSeltasIsland,
        LookAtPomPomIsland,
        DropBuilding,
        NumberTicker,
        ConfirmButton,
        VoteSusButton,
        NextIsland,
        MainMenuNavButtons,
        MainMenuNavHover,
        MainMenuGameStart,
        PopUpCloses,
        TimerTicking_SusVersion,
        GoodWins_SusVersion,
        SusWins_SusVersion,
        RoundStart_SusVersion,
        RoundEnd_SusVersion,
        SusVote_start_SusVersion,
        SusVoteEnd_SusVersion,
        BuildVoteStart_SusVersion,
        BuildVoteEnd_SusVersion
    }

    [Header("Click Sound")]
    [SerializeField]
    private SFXType soundEffect = SFXType.GeneralButtonClick;

    [Header("Optional: Hover Sound")]
    [SerializeField]
    private bool playHoverSound = false;

    [SerializeField]
    private SFXType hoverSoundEffect = SFXType.MainMenuNavHover;

    private bool hasHovered = false;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        AudioClip clip = GetClipFromEnum(soundEffect, SoundAndMusicController.Instance.SfxClips);
        if (clip != null)
        {
            SoundAndMusicController.Instance.PlaySFX(clip);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playHoverSound || hasHovered)
            return;

        AudioClip clip = GetClipFromEnum(hoverSoundEffect, SoundAndMusicController.Instance.SfxClips);
        if (clip != null)
        {
            SoundAndMusicController.Instance.PlaySFX(clip);
            hasHovered = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hasHovered = false;
    }

    private AudioClip GetClipFromEnum(SFXType type, SFXClipCollection sfx)
    {
        return type switch
        {
            SFXType.GeneralButtonClick => sfx.generalButtonClick,
            SFXType.PopupOpens => sfx.popupOpens,
            SFXType.BumbiJoins => sfx.bumbiJoins,
            SFXType.BumbiLeaves => sfx.bumbiLeaves,
            SFXType.LykiJoins => sfx.lykiJoins,
            SFXType.LykiLeaves => sfx.lykiLeaves,
            SFXType.PigynJoins => sfx.pigynJoins,
            SFXType.PigynLeaves => sfx.pigynLeaves,
            SFXType.GumbiJoins => sfx.gumbiJoins,
            SFXType.GumbiLeaves => sfx.gumbiLeaves,
            SFXType.SeltasJoins => sfx.seltasJoins,
            SFXType.SeltasLeaves => sfx.seltasLeaves,
            SFXType.PomPomJoins => sfx.pomPomJoins,
            SFXType.PomPomLeaves => sfx.pomPomLeaves,
            SFXType.GoodSideWins => sfx.goodSideWins,
            SFXType.SusSideWins => sfx.susSideWins,
            SFXType.RoundStart => sfx.roundStart,
            SFXType.RoundEnd => sfx.roundEnd,
            SFXType.BuildVotingTimeStart => sfx.buildVotingTimeStart,
            SFXType.BuildVotingTimeEnd => sfx.buildVotingTimeEnd,
            SFXType.SusVotingTimeStart => sfx.susVotingTimeStart,
            SFXType.SusVotingTimeEnd => sfx.susVotingTimeEnd,
            SFXType.TimerTicking => sfx.timerTicking,
            SFXType.LookAtBumbiIsland => sfx.lookAtBumbiIsland,
            SFXType.LookAtLykiIsland => sfx.lookAtLykiIsland,
            SFXType.LookAtPigynIsland => sfx.lookAtPigynIsland,
            SFXType.LookAtGumbiIsland => sfx.lookAtGumbiIsland,
            SFXType.LookAtSeltasIsland => sfx.lookAtSeltasIsland,
            SFXType.LookAtPomPomIsland => sfx.lookAtPomPomIsland,
            SFXType.DropBuilding => sfx.dropBuilding,
            SFXType.NumberTicker => sfx.numberTicker,
            SFXType.ConfirmButton => sfx.confirmButton,
            SFXType.VoteSusButton => sfx.voteSusButton,
            SFXType.NextIsland => sfx.nextIsland,
            SFXType.MainMenuNavButtons => sfx.mainMenuNavButtons,
            SFXType.MainMenuNavHover => sfx.MainMenuNavHover,
            SFXType.MainMenuGameStart => sfx.MainMenuGameStart,
            SFXType.PopUpCloses => sfx.PopUpCloses,
            SFXType.TimerTicking_SusVersion => sfx.TimerTicking_SusVersion,
            SFXType.GoodWins_SusVersion => sfx.GoodWins_SusVersion,
            SFXType.SusWins_SusVersion => sfx.SusWins_SusVersion,
            SFXType.RoundStart_SusVersion => sfx.RoundStart_SusVersion,
            SFXType.RoundEnd_SusVersion => sfx.RoundEnd_SusVersion,
            SFXType.SusVote_start_SusVersion => sfx.SusVote_start_SusVersion,
            SFXType.SusVoteEnd_SusVersion => sfx.SusVoteEnd_SusVersion,
            SFXType.BuildVoteStart_SusVersion => sfx.BuildVoteStart_SusVersion,
            SFXType.BuildVoteEnd_SusVersion => sfx.BuildVoteEnd_SusVersion,
            _ => null
        };
    }
}
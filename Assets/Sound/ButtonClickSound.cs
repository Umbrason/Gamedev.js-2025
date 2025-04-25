using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum SFXType
    {
        _02_SharedGoalProgresses,
        _03_GeneralButtonClicks,
        _04_PopUpOpens,
        _05_BumbiJoinsLobby,
        _06_BumbiLeavesLobby,
        _07_LykiJoinsLobby,
        _08_LykiLeavesLobby,
        _09_PigynJoinsLobby,
        _10_PigynLeavesLobby,
        _11_GumbiJoinsLobby,
        _12_GumbiLeavesLobby,
        _13_SeltasJoinsLobby,
        _14_SeltasLeavesLobby,
        _15_PomPomJoinsLobby,
        _16_PomPomLeavesLobby,
        _17_GoodWins_GoodVersion,
        _18_SusWins_GoodVersion,
        _19_RoundStart_GoodVersion,
        _20_RoundEnd_GoodVersion,
        _21_BuildVoteStart_GoodVersion,
        _22_BuildVoteEnd_GoodVersion,
        _23_SusVoteStart_GoodVersion,
        _24_SusVoteEnd_GoodVersion,
        _25_TimerTicking_GoodVersion,
        _26_CreaturesWalkingAround,
        _27_LookAtBumbiIsland,
        _28_LookAtLykiIsland,
        _29_LookAtPigynIsland,
        _30_LookAtGumbiIsland,
        _31_LookAtSeltasIsland,
        _32_LookAtPomPomIsland,
        _33_DropBuildingOnMap,
        _34_OpeningDropdown,
        _35_NumberTicker,
        _36_ConfirmButton,
        _37_VoteSusButton,
        _38_NextIsland,
        _39_PauseButton,
        _40_MainMenuNavigationButtons,
        _41_MainMenuNavHover,
        _42_MainMenuGameStart,
        _43_PopUpCloses,
        _44_TimerTicking_SusVersion,
        _45_GoodWins_SusVersion,
        _46_SusWins_SusVersion,
        _47_RoundStart_SusVersion,
        _48_RoundEnd_SusVersion,
        _49_SusVoteStart_SusVersion,
        _50_SusVoteEnd_SusVersion,
        _51_BuildVoteStart_SusVersion,
        _52_BuildVoteEnd_SusVersion
    }

    [Header("Click Sound")]
    [SerializeField] private SFXType soundEffect = SFXType._03_GeneralButtonClicks;

    [Header("Optional: Hover Sound")]
    [SerializeField] private bool playHoverSound = false;
    [SerializeField] private SFXType hoverSoundEffect = SFXType._41_MainMenuNavHover;

    private bool hasHovered = false;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        SoundAndMusicController.Instance.PlaySFX(GetClipFromEnum(soundEffect, SoundAndMusicController.Instance.SfxClips), PlayerID.None);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playHoverSound || hasHovered) return;

        SoundAndMusicController.Instance.PlaySFX(GetClipFromEnum(hoverSoundEffect, SoundAndMusicController.Instance.SfxClips), PlayerID.None);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hasHovered = false;
    }

    private AudioClip GetClipFromEnum(SFXType type, SFXClipCollection sfxClipCollection)
    {
        return type switch
        {
            SFXType._02_SharedGoalProgresses => sfxClipCollection._02_SharedGoalProgresses,
            SFXType._03_GeneralButtonClicks => sfxClipCollection._03_GeneralButtonClicks,
            SFXType._04_PopUpOpens => sfxClipCollection._04_PopUpOpens,
            SFXType._05_BumbiJoinsLobby => sfxClipCollection._05_BumbiJoinsLobby,
            SFXType._06_BumbiLeavesLobby => sfxClipCollection._06_BumbiLeavesLobby,
            SFXType._07_LykiJoinsLobby => sfxClipCollection._07_LykiJoinsLobby,
            SFXType._08_LykiLeavesLobby => sfxClipCollection._08_LykiLeavesLobby,
            SFXType._09_PigynJoinsLobby => sfxClipCollection._09_PigynJoinsLobby,
            SFXType._10_PigynLeavesLobby => sfxClipCollection._10_PigynLeavesLobby,
            SFXType._11_GumbiJoinsLobby => sfxClipCollection._11_GumbiJoinsLobby,
            SFXType._12_GumbiLeavesLobby => sfxClipCollection._12_GumbiLeavesLobby,
            SFXType._13_SeltasJoinsLobby => sfxClipCollection._13_SeltasJoinsLobby,
            SFXType._14_SeltasLeavesLobby => sfxClipCollection._14_SeltasLeavesLobby,
            SFXType._15_PomPomJoinsLobby => sfxClipCollection._15_PomPomJoinsLobby,
            SFXType._16_PomPomLeavesLobby => sfxClipCollection._16_PomPomLeavesLobby,
            SFXType._17_GoodWins_GoodVersion => sfxClipCollection._17_GoodWins_GoodVersion,
            SFXType._18_SusWins_GoodVersion => sfxClipCollection._18_SusWins_GoodVersion,
            SFXType._19_RoundStart_GoodVersion => sfxClipCollection._19_RoundStart_GoodVersion,
            SFXType._20_RoundEnd_GoodVersion => sfxClipCollection._20_RoundEnd_GoodVersion,
            SFXType._21_BuildVoteStart_GoodVersion => sfxClipCollection._21_BuildVoteStart_GoodVersion,
            SFXType._22_BuildVoteEnd_GoodVersion => sfxClipCollection._22_BuildVoteEnd_GoodVersion,
            SFXType._23_SusVoteStart_GoodVersion => sfxClipCollection._23_SusVoteStart_GoodVersion,
            SFXType._24_SusVoteEnd_GoodVersion => sfxClipCollection._24_SusVoteEnd_GoodVersion,
            SFXType._25_TimerTicking_GoodVersion => sfxClipCollection._25_TimerTicking_GoodVersion,
            SFXType._26_CreaturesWalkingAround => sfxClipCollection._26_CreaturesWalkingAround,
            SFXType._27_LookAtBumbiIsland => sfxClipCollection._27_LookAtBumbiIsland,
            SFXType._28_LookAtLykiIsland => sfxClipCollection._28_LookAtLykiIsland,
            SFXType._29_LookAtPigynIsland => sfxClipCollection._29_LookAtPigynIsland,
            SFXType._30_LookAtGumbiIsland => sfxClipCollection._30_LookAtGumbiIsland,
            SFXType._31_LookAtSeltasIsland => sfxClipCollection._31_LookAtSeltasIsland,
            SFXType._32_LookAtPomPomIsland => sfxClipCollection._32_LookAtPomPomIsland,
            SFXType._33_DropBuildingOnMap => sfxClipCollection._33_DropBuildingOnMap,
            SFXType._34_OpeningDropdown => sfxClipCollection._34_OpeningDropdown,
            SFXType._35_NumberTicker => sfxClipCollection._35_NumberTicker,
            SFXType._36_ConfirmButton => sfxClipCollection._36_ConfirmButton,
            SFXType._37_VoteSusButton => sfxClipCollection._37_VoteSusButton,
            SFXType._38_NextIsland => sfxClipCollection._38_NextIsland,
            SFXType._39_PauseButton => sfxClipCollection._39_PauseButton,
            SFXType._40_MainMenuNavigationButtons => sfxClipCollection._40_MainMenuNavigationButtons,
            SFXType._41_MainMenuNavHover => sfxClipCollection._41_MainMenuNavHover,
            SFXType._42_MainMenuGameStart => sfxClipCollection._42_MainMenuGameStart,
            SFXType._43_PopUpCloses => sfxClipCollection._43_PopUpCloses,
            SFXType._44_TimerTicking_SusVersion => sfxClipCollection._44_TimerTicking_SusVersion,
            SFXType._45_GoodWins_SusVersion => sfxClipCollection._45_GoodWins_SusVersion,
            SFXType._46_SusWins_SusVersion => sfxClipCollection._46_SusWins_SusVersion,
            SFXType._47_RoundStart_SusVersion => sfxClipCollection._47_RoundStart_SusVersion,
            SFXType._48_RoundEnd_SusVersion => sfxClipCollection._48_RoundEnd_SusVersion,
            SFXType._49_SusVoteStart_SusVersion => sfxClipCollection._49_SusVoteStart_SusVersion,
            SFXType._50_SusVoteEnd_SusVersion => sfxClipCollection._50_SusVoteEnd_SusVersion,
            SFXType._51_BuildVoteStart_SusVersion => sfxClipCollection._51_BuildVoteStart_SusVersion,
            SFXType._52_BuildVoteEnd_SusVersion => sfxClipCollection._52_BuildVoteEnd_SusVersion,
            _ => null
        };
    }
}
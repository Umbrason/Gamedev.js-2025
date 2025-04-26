using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundAndMusicController : Singleton<SoundAndMusicController>
{
    [SerializeField]
    private MusicClipCollection musicClips;
    public MusicClipCollection MusicClips { get => musicClips; }
    [SerializeField]
    private SFXClipCollection sfxClips;
    public SFXClipCollection SfxClips { get => sfxClips; }

    private AudioSource musicSource;
    private AudioSource ambienceSource;
    private AudioSource sfxSource;

    private void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        ambienceSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        ambienceSource.loop = true;
        sfxSource.loop = false;

        musicSource.playOnAwake = false;
        ambienceSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        EnsureSingleAudioListener();
    }

    public void PlayMusic(MusicType sfx, PlayerID id)
    {
        if (!CanPlay(id)) return;
        AudioClip clip = GetMusicClipFromEnum(sfx);
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayAmbience(MusicType sfx, PlayerID id)
    {
        if (!CanPlay(id)) return;
        AudioClip clip = GetMusicClipFromEnum(sfx);
        if (clip == null)
        {
            ambienceSource.Stop();
        }
        ambienceSource.clip = clip;
        ambienceSource.Play();
    }

    public void PlaySFX(SFXType sfx, PlayerID id)
    {
        if (!CanPlay(id)) return;
        AudioClip clip = GetSFXClipFromEnum(sfx);
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    private bool CanPlay(PlayerID id)
    {
        if (id == PlayerID.None) return true;
        if (GameNetworkManager.Instance != null)
        {
            return id == GameNetworkManager.Instance.PlayerID;
        }
        return true;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void EnsureSingleAudioListener()
    {
        AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();

        if (audioListeners.Length > 1)
        {
            for (int i = 1; i < audioListeners.Length; i++)
            {
                Destroy(audioListeners[i]);
            }
        }

        if (audioListeners.Length == 0)
        {
            gameObject.AddComponent<AudioListener>();
        }
    }

    private AudioClip GetSFXClipFromEnum(SFXType type)
    {
        return type switch
        {
            SFXType._02_SharedGoalProgresses => sfxClips._02_SharedGoalProgresses,
            SFXType._03_GeneralButtonClicks => sfxClips._03_GeneralButtonClicks,
            SFXType._04_PopUpOpens => sfxClips._04_PopUpOpens,
            SFXType._05_BumbiJoinsLobby => sfxClips._05_BumbiJoinsLobby,
            SFXType._06_BumbiLeavesLobby => sfxClips._06_BumbiLeavesLobby,
            SFXType._07_LykiJoinsLobby => sfxClips._07_LykiJoinsLobby,
            SFXType._08_LykiLeavesLobby => sfxClips._08_LykiLeavesLobby,
            SFXType._09_PigynJoinsLobby => sfxClips._09_PigynJoinsLobby,
            SFXType._10_PigynLeavesLobby => sfxClips._10_PigynLeavesLobby,
            SFXType._11_GumbiJoinsLobby => sfxClips._11_GumbiJoinsLobby,
            SFXType._12_GumbiLeavesLobby => sfxClips._12_GumbiLeavesLobby,
            SFXType._13_SeltasJoinsLobby => sfxClips._13_SeltasJoinsLobby,
            SFXType._14_SeltasLeavesLobby => sfxClips._14_SeltasLeavesLobby,
            SFXType._15_PomPomJoinsLobby => sfxClips._15_PomPomJoinsLobby,
            SFXType._16_PomPomLeavesLobby => sfxClips._16_PomPomLeavesLobby,
            SFXType._17_GoodWins_GoodVersion => sfxClips._17_GoodWins_GoodVersion,
            SFXType._18_SusWins_GoodVersion => sfxClips._18_SusWins_GoodVersion,
            SFXType._19_RoundStart_GoodVersion => sfxClips._19_RoundStart_GoodVersion,
            SFXType._20_RoundEnd_GoodVersion => sfxClips._20_RoundEnd_GoodVersion,
            SFXType._21_BuildVoteStart_GoodVersion => sfxClips._21_BuildVoteStart_GoodVersion,
            SFXType._22_BuildVoteEnd_GoodVersion => sfxClips._22_BuildVoteEnd_GoodVersion,
            SFXType._23_SusVoteStart_GoodVersion => sfxClips._23_SusVoteStart_GoodVersion,
            SFXType._24_SusVoteEnd_GoodVersion => sfxClips._24_SusVoteEnd_GoodVersion,
            SFXType._25_TimerTicking_GoodVersion => sfxClips._25_TimerTicking_GoodVersion,
            SFXType._26_CreaturesWalkingAround => sfxClips._26_CreaturesWalkingAround,
            SFXType._27_LookAtBumbiIsland => sfxClips._27_LookAtBumbiIsland,
            SFXType._28_LookAtLykiIsland => sfxClips._28_LookAtLykiIsland,
            SFXType._29_LookAtPigynIsland => sfxClips._29_LookAtPigynIsland,
            SFXType._30_LookAtGumbiIsland => sfxClips._30_LookAtGumbiIsland,
            SFXType._31_LookAtSeltasIsland => sfxClips._31_LookAtSeltasIsland,
            SFXType._32_LookAtPomPomIsland => sfxClips._32_LookAtPomPomIsland,
            SFXType._33_DropBuildingOnMap => sfxClips._33_DropBuildingOnMap,
            SFXType._34_OpeningDropdown => sfxClips._34_OpeningDropdown,
            SFXType._35_NumberTicker => sfxClips._35_NumberTicker,
            SFXType._36_ConfirmButton => sfxClips._36_ConfirmButton,
            SFXType._37_VoteSusButton => sfxClips._37_VoteSusButton,
            SFXType._38_NextIsland => sfxClips._38_NextIsland,
            SFXType._39_PauseButton => sfxClips._39_PauseButton,
            SFXType._40_MainMenuNavigationButtons => sfxClips._40_MainMenuNavigationButtons,
            SFXType._41_MainMenuNavHover => sfxClips._41_MainMenuNavHover,
            SFXType._42_MainMenuGameStart => sfxClips._42_MainMenuGameStart,
            SFXType._43_PopUpCloses => sfxClips._43_PopUpCloses,
            SFXType._44_TimerTicking_SusVersion => sfxClips._44_TimerTicking_SusVersion,
            SFXType._45_GoodWins_SusVersion => sfxClips._45_GoodWins_SusVersion,
            SFXType._46_SusWins_SusVersion => sfxClips._46_SusWins_SusVersion,
            SFXType._47_RoundStart_SusVersion => sfxClips._47_RoundStart_SusVersion,
            SFXType._48_RoundEnd_SusVersion => sfxClips._48_RoundEnd_SusVersion,
            SFXType._49_SusVoteStart_SusVersion => sfxClips._49_SusVoteStart_SusVersion,
            SFXType._50_SusVoteEnd_SusVersion => sfxClips._50_SusVoteEnd_SusVersion,
            SFXType._51_BuildVoteStart_SusVersion => sfxClips._51_BuildVoteStart_SusVersion,
            SFXType._52_BuildVoteEnd_SusVersion => sfxClips._52_BuildVoteEnd_SusVersion,
            _ => null
        };
    }

    private AudioClip GetMusicClipFromEnum(MusicType type)
    {
        return type switch
        {
            MusicType.ambienceGoodLoopable => musicClips.ambienceGoodLoopable,
            MusicType.ambienceSusLoopable => musicClips.ambienceSusLoopable,
            MusicType.mainMenuLoopable => musicClips.mainMenuLoopable,
            MusicType.soundtrackGoodLoopable => musicClips.soundtrackGoodLoopable,
            MusicType.soundtrackSusLoopable => musicClips.soundtrackSusLoopable,
            _ => null
        };
    }
}
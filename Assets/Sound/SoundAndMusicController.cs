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

    public void PlayMusic(AudioClip clip, PlayerID id)
    {
        if (!CanPlay(id)) return;
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayAmbience(AudioClip clip, PlayerID id)
    {
        if (!CanPlay(id)) return;
        if (clip == null)
        {
            ambienceSource.Stop();
        }
        ambienceSource.clip = clip;
        ambienceSource.Play();
    }

    public void PlaySFX(AudioClip clip, PlayerID id)
    {
        if (!CanPlay(id)) return;
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
}
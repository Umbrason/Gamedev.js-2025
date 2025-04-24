using UnityEngine;

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

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayAmbience(AudioClip clip)
    {
        if (clip == null) return;
        ambienceSource.clip = clip;
        ambienceSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
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

        // Wenn kein AudioListener vorhanden ist, einen hinzufügen (optional)
        if (audioListeners.Length == 0)
        {
            gameObject.AddComponent<AudioListener>();
        }
    }
}
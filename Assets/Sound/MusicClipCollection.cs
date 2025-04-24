using UnityEngine;

[CreateAssetMenu(fileName = "MusicClips", menuName = "Audio/MusicClipCollection")]
public class MusicClipCollection : ScriptableObject
{
    public AudioClip ambienceGoodLoopable;
    public AudioClip ambienceSusLoopable;
    public AudioClip soundtrackGoodLoopable;
    public AudioClip soundtrackSusLoopable;
}
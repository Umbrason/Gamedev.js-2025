using UnityEngine;

public interface ITimedPhase
{
    public float TimeRemaining { get; }
    public float Duration { get; }
    public float Progress => Duration != 0 ? Mathf.Clamp01(1f - TimeRemaining / Duration) : 1;
    public bool CanSkip();
    public void Skip();
}

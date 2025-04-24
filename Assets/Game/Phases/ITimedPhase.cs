using UnityEngine;

public interface ITimedPhase
{
    public float TimeRemaining { get; }
    public float Duration { get; }
    public float Progress => Mathf.Clamp01(1f - TimeRemaining / Duration);
    public bool CanSkip();
    public void Skip();
}

using System;
using UnityEngine;
using UnityEngine.Events;

public class PhaseCountdown : MonoBehaviour
{
    [SerializeField] GameInstance gameInstance;
    [SerializeField] UnityEvent<bool> IsActive;
    [SerializeField] UnityEvent<float> Progress;
    [SerializeField] UnityEvent<string> TimeRemaining;


    ITimedPhase _phase;


    private void Start()
    {
        if(gameInstance == null)
        {
            Debug.LogError("gameInstance is null");
            return;
        }
        gameInstance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnPhaseChanged(IGamePhase phase)
    {
        _phase = phase as ITimedPhase;

        IsActive?.Invoke(_phase != null);
    }

    private void Update()
    {
        if (_phase == null) return;

        Progress.Invoke(_phase.Progress);
        TimeRemaining.Invoke(Mathf.Max(Mathf.CeilToInt(_phase.TimeRemaining), 0).ToString());
    }
}

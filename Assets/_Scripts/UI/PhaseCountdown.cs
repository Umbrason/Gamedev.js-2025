using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhaseCountdown : MonoBehaviour
{
    [SerializeField] private GameInstance gameInstance;
    [SerializeField] private UnityEvent<bool> IsActive;
    [SerializeField] private UnityEvent<float> Progress;
    [SerializeField] private UnityEvent<string> TimeRemaining;
    [SerializeField] private Button SkipButton;

    ITimedPhase _phase;


    private void Start()
    {
        if (gameInstance == null)
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
        SkipButton.gameObject.SetActive(_phase?.CanSkip() ?? false);
    }

    private void Update()
    {
        if (_phase == null) return;
        Progress.Invoke(_phase.Progress);
        TimeRemaining.Invoke(Mathf.Max(Mathf.CeilToInt(_phase.TimeRemaining), 0).ToString());
    }

    public void SkipPhase()
    {
        if (_phase?.CanSkip() ?? false)
            _phase?.Skip();
    }
}

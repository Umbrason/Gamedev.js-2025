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

    private ITimedPhase _phase;
    private int _lastDisplayedTime = -1;

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
        _lastDisplayedTime = -1;
    }

    private void Update()
    {
        if (_phase == null) return;

        if (GameNetworkManager.Instance.GamePausedBecauseOfDisconnect)
        {
            _phase.startTime += Time.unscaledDeltaTime;
        }

        Progress.Invoke(_phase.Progress);

        int currentTime = Mathf.Max(Mathf.CeilToInt(_phase.TimeRemaining), 0);

        if (currentTime != _lastDisplayedTime)
        {
            _lastDisplayedTime = currentTime;
            OnTimeChanged(currentTime);
        }

        TimeRemaining.Invoke(currentTime.ToString());
    }

    private void OnTimeChanged(int newTime)
    {
        if (newTime <= 5)
        {
            switch (gameInstance.ClientPlayerData.Role)
            {
                case PlayerRole.Balanced: SoundAndMusicController.Instance.PlaySFX(SFXType._25_TimerTicking_GoodVersion, gameInstance.ClientID); break;
                case PlayerRole.Selfish: SoundAndMusicController.Instance.PlaySFX(SFXType._44_TimerTicking_SusVersion, gameInstance.ClientID); break;
            }
        }
    }

    public void SkipPhase()
    {
        if (_phase?.CanSkip() ?? false)
        {
            _phase?.Skip();
        }
    }
}
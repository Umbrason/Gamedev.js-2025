using System.Collections;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [SerializeField] private GameInstance GameInstance;

    [SerializeField] private AIConfigData config;

    private Coroutine _playingPhaseCoroutine;

    private void OnEnable()
    {
        if(GameInstance == null)
        {
            Debug.LogError("Missing GameInstance");
            return;
        }

        if (config == null)
        {
            Debug.LogError("Missing AIConfig");
            return;
        }

        GameInstance.OnPhaseChanged += OnGamePhaseChanged;

        OnGamePhaseChanged(GameInstance.CurrentPhase);
    }

    private void OnDisable()
    {
        if (GameInstance == null) return;

        GameInstance.OnPhaseChanged -= OnGamePhaseChanged;

        if(_playingPhaseCoroutine != null) StopCoroutine(_playingPhaseCoroutine);
    }

    private void OnGamePhaseChanged(IGamePhase phase)
    {
        if (_playingPhaseCoroutine != null) StopCoroutine(_playingPhaseCoroutine);

        IEnumerator playingPhase = config.PlayingPhase(phase, GameInstance);

        if(playingPhase != null)
            _playingPhaseCoroutine = StartCoroutine(playingPhase);
    }
}

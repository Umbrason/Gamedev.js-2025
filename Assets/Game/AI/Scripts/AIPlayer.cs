using System.Collections;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [SerializeField] private GameInstance gameInstance;

    [SerializeField] private AIConfigData config;

    public GameInstance GameInstance => gameInstance;
    public AIConfigData Config => config;

    private Coroutine _playingPhaseCoroutine;

    public bool accused = false;

    private void OnEnable()
    {
        if(gameInstance == null)
        {
            Debug.LogError("Missing GameInstance");
            return;
        }

        if (config == null)
        {
            Debug.LogError("Missing AIConfig");
            return;
        }

        gameInstance.OnPhaseChanged += OnGamePhaseChanged;

        OnGamePhaseChanged(gameInstance.CurrentPhase);
    }

    private void OnDisable()
    {
        if (gameInstance == null) return;

        gameInstance.OnPhaseChanged -= OnGamePhaseChanged;

        if(_playingPhaseCoroutine != null) StopCoroutine(_playingPhaseCoroutine);
    }

    private void OnGamePhaseChanged(IGamePhase phase)
    {
        if (_playingPhaseCoroutine != null) StopCoroutine(_playingPhaseCoroutine);

        IEnumerator playingPhase = config.PlayingPhase(phase, this);

        if(playingPhase != null)
            _playingPhaseCoroutine = StartCoroutine(playingPhase);
    }

    public void Log(string message)
    {
        if (!config.LogActions) return;
        Debug.Log(string.Format("AI {0} {1} ", gameInstance.ClientID, message));
    }
}

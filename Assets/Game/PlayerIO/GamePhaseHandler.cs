using UnityEngine;

public abstract class GamePhaseHandler<T> : MonoBehaviour where T : IGamePhase
{
    [field: SerializeField] protected GameInstance Game { get; private set; }
    protected T Phase { get; private set; }


    void Awake()
    {
        Game.OnPhaseChanged += OnGamePhaseChanged;
        OnGamePhaseChanged(Game.CurrentPhase);
    }

    void OnDestroy()
    {
        Game.OnPhaseChanged -= OnGamePhaseChanged;
    }

    private bool active = false;
    void OnGamePhaseChanged(IGamePhase phase)
    {
        var newPhase = phase is T t ? t : default;
        if (!active && phase is T)
        {
            Phase = newPhase;
            OnPhaseEntered();
        }
        else if (active)
        {
            OnPhaseExited();
            Phase = newPhase;
        }
        active = phase is T;
    }

    public virtual void OnPhaseEntered() { }
    public virtual void OnPhaseExited() { }
}
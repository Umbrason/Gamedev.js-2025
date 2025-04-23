using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceChangeAnimation : MonoBehaviour
{
    [Header("Scene Refs")]
    [SerializeField] private PlayerIDButtons playerButtons;
    [SerializeField] private PlayerDisplay playerDisplay;
    [SerializeField] private GameInstance gameInstance;
    [SerializeField] private MissionsDisplay missions;
    [Header("Asset Refs")]
    [SerializeField] private ResourceSpriteLib ResourceIcons;
    [SerializeField] private SpriteRenderer ResourceParticleTemplate;
    const float DefaultAnimationDuration = 1.5f;
    const float SpawnDelay = .2f;

    private struct ChangeAnimationInstance
    {
        public ChangeAnimationInstance(SpriteRenderer spriteInstance, Vector3 start, Vector3 end, Vector3 arcDirection, float duration, Action onStart, Action onEnd)
        {
            SpriteInstance = spriteInstance;
            StartPos = start;
            EndPos = end;
            ArcDirection = arcDirection;
            StartTime = Time.time;
            Duration = duration;
            OnStart = onStart;
            OnEnd = onEnd;
        }

        public void DoStart()
        {
            SpriteInstance.gameObject.SetActive(true);
            StartTime = Time.time;
        }
        public Action OnStart { get; private set; }
        public Action OnEnd { get; private set; }
        public SpriteRenderer SpriteInstance { get; private set; }
        public Vector3 StartPos { get; private set; }
        public Vector3 EndPos { get; private set; }
        public Vector3 ArcDirection { get; private set; }
        public float StartTime { get; private set; }
        public float Duration { get; private set; }
        public readonly bool Expired => (Time.time - StartTime) > Duration;

        public readonly void UpdateSprite()
        {
            var t = Mathf.Clamp01((Time.time - StartTime) / Duration);
            //t = 1 - (1 - t) * (1 - t); //styling the time function
            //t = (.5f + (t * 2 - 1) * (t * 2 - 1) * (t * 2 - 1) * .5f + t * 2) / 3f;
            t = t * t;

            var invT = 1 - t;
            var amplitude = invT * invT - invT * invT * invT;
            amplitude /= 0.148148f;
            SpriteInstance.transform.position = StartPos * invT + EndPos * t + ArcDirection * amplitude;
        }
    }
    private readonly List<ChangeAnimationInstance> instances = new();

    float lastSpawn = -SpawnDelay;
    void Update()
    {
        if (Time.time - lastSpawn > SpawnDelay && spawnQueue.Count > 0)
        {
            var instance = spawnQueue.Dequeue();
            instances.Add(instance);
            instance.DoStart();
            lastSpawn = Time.time;
        }
        foreach (var instance in instances)
            instance.UpdateSprite();
        var expired = instances.Where((i) => i.Expired);
        foreach (var item in expired.ToArray())
            Despawn(item);
    }


    private readonly Queue<SpriteRenderer> pool = new();


    private Vector3 ScreenPosToWorldTarget(Vector2 screenPos)
        => Camera.main.ScreenToWorldPoint((Vector3)screenPos + Vector3.forward * 10f);
    private Vector3 PositionOf(PlayerID player)
    {
        var faction = gameInstance.PlayerData?.GetValueOrDefault(player)?.Faction ?? PlayerFaction.None;
        var slot = playerDisplay.SlotOf(faction);
        if (slot) return slot.transform.position + Vector3.up * 1f;
        var playerButton = playerButtons?.ButtonOf(player);
        if (!playerButton) return default;
        var screenPos = playerButton.GetComponent<RectTransform>().ScreenPosition();
        return ScreenPosToWorldTarget(screenPos);
    }
    private Vector3 PositionOf(SharedGoalID goalID)
    {
        var button = missions.ButtonOf(goalID);
        var screenPos = button.GetComponent<RectTransform>().ScreenPosition();
        return ScreenPosToWorldTarget(screenPos);
    }
    private Vector3 PositionOf(HexPosition position) => position.WorldPositionCenter + Vector3.up * 1f;

    #region PlayerID <-> SharedGoalID
    public void Spawn(Resource resource, PlayerID start, SharedGoalID end, float duration = DefaultAnimationDuration, Action onStart = null, Action onEnd = null) => Spawn(resource, PositionOf(start), PositionOf(end), duration, onStart, onEnd);
    public void Spawn(Resource resource, SharedGoalID start, PlayerID end, float duration = DefaultAnimationDuration, Action onStart = null, Action onEnd = null) => Spawn(resource, PositionOf(start), PositionOf(end), duration, onStart, onEnd);
    #endregion

    #region  PlayerID <-> PlayerID
    public void Spawn(Resource resource, PlayerID start, PlayerID end, float duration = DefaultAnimationDuration, Action onStart = null, Action onEnd = null) => Spawn(resource, PositionOf(start), PositionOf(end), duration, onStart, onEnd);
    #endregion

    #region HexPosition <-> PlayerID
    public void Spawn(Resource resource, PlayerID start, HexPosition end, float duration = DefaultAnimationDuration, Action onStart = null, Action onEnd = null) => Spawn(resource, PositionOf(start), PositionOf(end), duration, onStart, onEnd);
    public void Spawn(Resource resource, HexPosition start, PlayerID end, float duration = DefaultAnimationDuration, Action onStart = null, Action onEnd = null) => Spawn(resource, PositionOf(start), PositionOf(end), duration, onStart, onEnd);
    #endregion


    readonly Queue<ChangeAnimationInstance> spawnQueue = new();
    public void Spawn(Resource resource, Vector3 start, Vector3 end, float duration = DefaultAnimationDuration, Action onStart = null, Action onEnd = null)
    {
        var SpriteInstance = pool.Count > 0 ? pool.Dequeue() : Instantiate(ResourceParticleTemplate, transform);
        SpriteInstance.sprite = ResourceIcons[resource];
        SpriteInstance.transform.position = start;

        var travelDirection = end - start;
        var arcDirection = Quaternion.AngleAxis(UnityEngine.Random.value * 180f, travelDirection) * Vector3.Cross(travelDirection, Vector3.up);
        arcDirection *= Mathf.Sign(arcDirection.y);
        arcDirection = arcDirection.normalized;
        arcDirection *= UnityEngine.Random.value * 3 + 1;

        var startOffset = UnityEngine.Random.insideUnitSphere;
        startOffset *= Mathf.Sign(startOffset.y);
        start += .5f * UnityEngine.Random.value * startOffset;

        var endOffset = UnityEngine.Random.insideUnitSphere;
        endOffset *= Mathf.Sign(endOffset.y);
        end += .5f * UnityEngine.Random.value * endOffset;

        duration *= 1f - ((UnityEngine.Random.value - .5f) * 2f) * .2f;
        spawnQueue.Enqueue(new ChangeAnimationInstance(SpriteInstance, start, end, arcDirection, duration, onStart, onEnd));
    }

    private void Despawn(ChangeAnimationInstance instance)
    {
        instance.SpriteInstance.gameObject.SetActive(false);
        instances.Remove(instance);
        pool.Enqueue(instance.SpriteInstance);
    }
}

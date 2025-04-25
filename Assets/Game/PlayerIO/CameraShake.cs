using System;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static Action<float> OnShake;
    public static void Shake(float strength)
    {
        OnShake?.Invoke(strength);
    }

    Spring.Config springConfig;
    BaseSpring RZSpring;
    void Awake() => RZSpring = new(springConfig);

    void OnEnable()
    {
        OnShake += ApplyShake;
    }

    void OnDisable()
    {
        OnShake -= ApplyShake;
    }

    void Update()
    {
        RZSpring.Step(Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0, 0, RZSpring.Position);
    }

    private void ApplyShake(float strength)
    {
        RZSpring.Velocity = strength * Mathf.Sign(RZSpring.Velocity + UnityEngine.Random.value);
    }
}

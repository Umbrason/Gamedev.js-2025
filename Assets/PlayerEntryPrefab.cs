using UnityEngine;

public class PlayerEntryPrefab : MonoBehaviour
{
    public float spawnScale = 0f;            // Start scale
    public float targetScale = 1f;           // Final scale
    public float animationTime = 0.5f;       // Duration of the animation
    public LeanTweenType easeType = LeanTweenType.easeOutBack;

    void OnEnable()
    {
        transform.localScale = Vector3.one * spawnScale;
        LeanTween.scale(gameObject, Vector3.one * targetScale, animationTime).setEase(easeType);
    }
}

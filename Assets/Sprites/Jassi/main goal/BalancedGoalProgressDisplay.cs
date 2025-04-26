using UnityEngine;
using UnityEngine.UI;

public class BalancedGoalProgressDisplay : MonoBehaviour
{
    Cached<Image> cached_ProgressImage;
    Image ProgressImage => cached_ProgressImage[this];
    [SerializeField] Sprite Sprite;
    [SerializeField] Texture2D Mask;
    [SerializeField] Material progressMaterial;
    [SerializeField] Material finishedMaterial;


    void OnEnable()
    {
        progressMaterial = Instantiate(progressMaterial);
        progressMaterial.SetTexture("_Mask", Mask);
        ProgressImage.sprite = Sprite;
        NormalizedProgress = NormalizedProgress;
    }

    private float normalizedProgress = 0;
    public float NormalizedProgress
    {
        get => normalizedProgress;
        set
        {
            normalizedProgress = value;
            var finished = value >= 1;
            ProgressImage.material = finished ? finishedMaterial : progressMaterial;
            if (finished) return;
            progressMaterial.SetFloat("_Progress", value);
        }
    }
}

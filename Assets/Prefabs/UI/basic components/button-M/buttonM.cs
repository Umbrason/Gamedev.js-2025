using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class buttonM : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Button Size")]
    private RectTransform rectTransform;
    private Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float animationTime = 0.1f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(rectTransform, originalScale * hoverScale, animationTime).setEaseOutBack();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(rectTransform, originalScale, animationTime).setEaseOutBack();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LeanTween.scale(rectTransform, originalScale * 0.95f, animationTime / 2).setEaseInQuad().setOnComplete(() =>
        {
            bool stillHovering = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition);
            Vector3 targetScale = stillHovering ? originalScale * hoverScale : originalScale;
            LeanTween.scale(rectTransform, targetScale, animationTime).setEaseOutBack();
        });
    }
}

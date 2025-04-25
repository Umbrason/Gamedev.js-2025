using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class buttonBack : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scaling")]
    private RectTransform rectTransform;
    private Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float animationEnterTime = 0.1f;
    public float animationExitTime = 0.1f;

    [Header("Background Fade")]
    public Image btnImage;
    public float fadeInAlpha = 1f;
    public float fadeOutAlpha = 0f;

    [Header("Text Animation")]
    public GameObject text;
    public float moveDistance = 10f;
    private Vector3 textOriginalPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        if (btnImage != null)
        {
            Color color = btnImage.color;
            color.a = fadeOutAlpha;
            btnImage.color = color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(rectTransform, originalScale * hoverScale, animationEnterTime).setEaseOutBack();

        if (btnImage != null)
        {
            LeanTween.alpha(btnImage.rectTransform, fadeInAlpha, animationEnterTime).setEaseOutQuad();
        }

        if (text != null)
        {
            Vector3 newPos = textOriginalPos + new Vector3(moveDistance, 0f, 0f);
            LeanTween.move(text.GetComponent<RectTransform>(), newPos, animationEnterTime).setEaseOutQuad();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(rectTransform, originalScale, animationExitTime).setEaseOutBack();

        if (btnImage != null)
        {
            LeanTween.alpha(btnImage.rectTransform, fadeOutAlpha, animationExitTime).setEaseOutQuad();
        }

        if (text != null)
        {
            LeanTween.move(text.GetComponent<RectTransform>(), textOriginalPos, animationExitTime).setEaseOutQuad();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LeanTween.scale(rectTransform, originalScale * 0.95f, animationExitTime / 2).setEaseInQuad().setOnComplete(() =>
        {
            bool stillHovering = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition);
            Vector3 targetScale = stillHovering ? originalScale * hoverScale : originalScale;
            LeanTween.scale(rectTransform, targetScale, animationExitTime).setEaseOutBack();
        });
    }
}


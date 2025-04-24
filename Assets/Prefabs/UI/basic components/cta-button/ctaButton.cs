using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ctaButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Button Size")]
    private RectTransform rectTransform;
    private Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float animationTime = 0.1f;

    [Header("Ornament Image")]
    public Image ornamentImage;
    public Sprite[] ornamentSprites;
    public float offset = 20f;

    private Vector3 ornamentStartPos;
    private int spriteIndex = 0;

    private bool isHovering = false;
    private bool isExiting = false;

    private float spriteChangeInterval = 0.1f;
    private float spriteChangeTimer = 0f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        if (ornamentImage != null)
        {
            ornamentStartPos = ornamentImage.rectTransform.anchoredPosition;
        }
    }

    void OnEnable()
    {
        LeanTween.reset();
    }

    void OnDisable()
    {
        LeanTween.cancel(gameObject);
    }

    void Update()
    {
        if (!isHovering && !isExiting) return;

        spriteChangeTimer += Time.deltaTime;
        if (spriteChangeTimer >= spriteChangeInterval && ornamentSprites.Length > 0)
        {
            spriteChangeTimer = 0f;
            spriteIndex += isHovering ? 1 : -1;
            spriteIndex = Mathf.Clamp(spriteIndex, 0, ornamentSprites.Length - 1);
            ornamentImage.sprite = ornamentSprites[spriteIndex];
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        isExiting = false;

        LeanTween.scale(rectTransform, originalScale * hoverScale, animationTime).setEaseOutBack();

        if (ornamentImage != null)
        {
            RectTransform rt = ornamentImage.rectTransform;
            LeanTween.move(rt, ornamentStartPos + new Vector3(offset, 0, 0), animationTime).setEaseOutBack();
            LeanTween.alpha(rt, 1f, animationTime).setEaseOutQuad();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isExiting = true;

        LeanTween.scale(rectTransform, originalScale, animationTime).setEaseOutBack();

        if (ornamentImage != null)
        {
            RectTransform rt = ornamentImage.rectTransform;
            LeanTween.move(rt, ornamentStartPos, animationTime).setEaseOutQuad();
            LeanTween.alpha(rt, 0f, animationTime).setEaseOutQuad();
        }
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
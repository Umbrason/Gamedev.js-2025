using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class inputfield : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Size")]
    private RectTransform rectTransform;
    private Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float animationTime = 0.1f;

    [Header("Button Ornamentation")]
    public Image leftImage;
    public Image rightImage;

    public Sprite[] leftSprites;
    public Sprite[] rightSprites;

    public float sideOffset = 20f;

    private Vector3 leftStartPos;
    private Vector3 rightStartPos;

    private bool isHovering = false;
    private bool isExiting = false;

    private float spriteChangeInterval = 0.1f;
    private float spriteChangeTimer = 0f;

    private int leftSpriteIndex = 0;
    private int rightSpriteIndex = 0;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        if (leftImage != null) leftStartPos = leftImage.rectTransform.anchoredPosition;
        if (rightImage != null) rightStartPos = rightImage.rectTransform.anchoredPosition;
    }

    void OnEnable()
    {
        LeanTween.reset();  // Stop all LeanTween animations when re-enabling the script
    }

    void OnDisable()
    {
        LeanTween.cancel(gameObject);  // Cancel LeanTween animations when the object is disabled
    }

    void Update()
    {
        if (isHovering)
        {
            spriteChangeTimer += Time.deltaTime;
            if (spriteChangeTimer >= spriteChangeInterval && leftSpriteIndex < leftSprites.Length)
            {
                spriteChangeTimer = 0f;
                UpdateSpriteSheet(leftImage, leftSprites, ref leftSpriteIndex, 1);
                UpdateSpriteSheet(rightImage, rightSprites, ref rightSpriteIndex, 1);
            }
        }
        else if (isExiting)
        {
            spriteChangeTimer += Time.deltaTime;
            if (spriteChangeTimer >= spriteChangeInterval && leftSpriteIndex > 0)
            {
                spriteChangeTimer = 0f;
                UpdateSpriteSheet(leftImage, leftSprites, ref leftSpriteIndex, -1);
                UpdateSpriteSheet(rightImage, rightSprites, ref rightSpriteIndex, -1);
            }
        }
    }

        private void UpdateSpriteSheet(Image image, Sprite[] sprites, ref int spriteIndex, int direction)
        {
            if (sprites.Length == 0) return;

            spriteIndex += direction;
            spriteIndex = Mathf.Clamp(spriteIndex, 0, sprites.Length - 1);
            image.sprite = sprites[spriteIndex];
        }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        isExiting = false;

        LeanTween.scale(rectTransform, originalScale * hoverScale, animationTime).setEaseOutBack();

        if (leftImage != null)
        {
            LeanTween.move(leftImage.rectTransform, leftStartPos + new Vector3(-sideOffset, 0, 0), animationTime).setEaseOutBack();
            LeanTween.scale(leftImage.rectTransform, Vector3.one * 1.2f, animationTime).setEaseOutBack();
        }

        if (rightImage != null)
        {
            LeanTween.move(rightImage.rectTransform, rightStartPos + new Vector3(sideOffset, 0, 0), animationTime).setEaseOutBack();
            LeanTween.scale(rightImage.rectTransform, Vector3.one * 1.2f, animationTime).setEaseOutBack();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isExiting = true;

        LeanTween.scale(rectTransform, originalScale, animationTime).setEaseOutBack();

        if (leftImage != null)
        {
            LeanTween.move(leftImage.rectTransform, leftStartPos, animationTime).setEaseOutQuad();
            LeanTween.scale(leftImage.rectTransform, Vector3.one, animationTime).setEaseOutBack();
        }

        if (rightImage != null)
        {
            LeanTween.move(rightImage.rectTransform, rightStartPos, animationTime).setEaseOutQuad();
            LeanTween.scale(rightImage.rectTransform, Vector3.one, animationTime).setEaseOutBack();
        }
    }
}
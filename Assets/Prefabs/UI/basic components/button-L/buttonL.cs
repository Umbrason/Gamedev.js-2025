using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class buttonL : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Button Size")]
    private RectTransform rectTransform;
    private Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float animationTime = 0.1f;

    [Header("Button Ornamentation")]
    public Image leftImage;  // Image component for left ornament
    public Image rightImage; // Image component for right ornament

    public Sprite[] leftSprites;  // Array of sprites for left image (sprite sheet)
    public Sprite[] rightSprites; // Array of sprites for right image (sprite sheet)

    public float sideOffset = 20f;

    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    
    private bool isHovering = false;
    private bool isExiting = false; // To track if we are in the exit state

    private float spriteChangeInterval = 0.1f;  // Interval for sprite sheet animation
    private float spriteChangeTimer = 0f;

    private int leftSpriteIndex = 0;
    private int rightSpriteIndex = 0;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        if (leftImage != null) leftStartPos = leftImage.rectTransform.anchoredPosition;
        if (rightImage != null) rightStartPos = rightImage.rectTransform.anchoredPosition;

        //make runes invisible at the start.
        Color currentColor = leftImage.color;
        currentColor.a = 0f;
        leftImage.color = currentColor;
        rightImage.color = currentColor;
    }

    void Update()
    {
        if (isHovering)
        {
            // Update the sprite for hover animation
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
            // Update the sprite for exit animation (reverse)
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

        // Update the sprite index based on the direction (1 for forward, -1 for backward)
        spriteIndex += direction;
        spriteIndex = Mathf.Clamp(spriteIndex, 0, sprites.Length - 1);

        // Apply the sprite to the image
        image.sprite = sprites[spriteIndex];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        isExiting = false; // Ensure exit state is not triggered during hover

        // Scale the button itself with LeanTween
        LeanTween.scale(rectTransform, originalScale * hoverScale, animationTime).setEaseOutBack();

        // Move and scale leftImage and rightImage
        if (leftImage != null)
        {
            LeanTween.move(leftImage.rectTransform, leftStartPos + new Vector3(-sideOffset, 0, 0), animationTime).setEaseOutBack();
            LeanTween.scale(leftImage.rectTransform, Vector3.one * 1.2f, animationTime).setEaseOutBack();

            // Animate the alpha of the left image from 0 to 1
            LeanTween.alpha(leftImage.rectTransform, 1f, animationTime).setEaseOutQuad();
        }

        if (rightImage != null)
        {
            LeanTween.move(rightImage.rectTransform, rightStartPos + new Vector3(sideOffset, 0, 0), animationTime).setEaseOutBack();
            LeanTween.scale(rightImage.rectTransform, Vector3.one * 1.2f, animationTime).setEaseOutBack();

            // Animate the alpha of the right image from 0 to 1
            LeanTween.alpha(rightImage.rectTransform, 1f, animationTime).setEaseOutQuad();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isExiting = true;

        // Reset the button scale back to original
        LeanTween.scale(rectTransform, originalScale, animationTime).setEaseOutBack();

        // Move and scale back the leftImage and rightImage
        if (leftImage != null)
        {
            LeanTween.move(leftImage.rectTransform, leftStartPos, animationTime).setEaseOutQuad();
            LeanTween.scale(leftImage.rectTransform, Vector3.one, animationTime).setEaseOutBack();

            // Animate the alpha of the left image from 1 to 0 (fade out)
            LeanTween.alpha(leftImage.rectTransform, 0f, animationTime).setEaseOutQuad();
        }

        if (rightImage != null)
        {
            LeanTween.move(rightImage.rectTransform, rightStartPos, animationTime).setEaseOutQuad();
            LeanTween.scale(rightImage.rectTransform, Vector3.one, animationTime).setEaseOutBack();

            // Animate the alpha of the right image from 1 to 0 (fade out)
            LeanTween.alpha(rightImage.rectTransform, 0f, animationTime).setEaseOutQuad();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Button scaling when clicked
        LeanTween.scale(rectTransform, originalScale * 0.95f, animationTime / 2).setEaseInQuad().setOnComplete(() =>
        {
            bool isHovering = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition);
            Vector3 targetScale = isHovering ? originalScale * hoverScale : originalScale;
            LeanTween.scale(rectTransform, targetScale, animationTime).setEaseOutBack();
        });

        // Optional: Add animation for the images on click if needed (e.g., scale down or move)
        if (leftImage != null)
        {
            LeanTween.scale(leftImage.rectTransform, Vector3.one * 1.1f, animationTime / 2).setEaseInQuad();
        }

        if (rightImage != null)
        {
            LeanTween.scale(rightImage.rectTransform, Vector3.one * 1.1f, animationTime / 2).setEaseInQuad();
        }
    }
}


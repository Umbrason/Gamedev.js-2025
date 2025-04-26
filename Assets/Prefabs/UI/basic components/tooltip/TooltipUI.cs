using UnityEngine;
using UnityEngine.UI;
using TMPro; // Only if you use TextMeshPro

public class TooltipUI : MonoBehaviour
{
    public TMP_Text tooltipText;

    public bool FollowMouse = true;
    public Vector2 offset = new Vector2(0, 50);

    void Update()
    {
        if (FollowMouse)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponent<RectTransform>(),
                Input.mousePosition,
                null,
                out pos);
            transform.localPosition = pos + offset;
        }
    }

    public void SetText(string text)
    {
        tooltipText.text = text;
    }
}


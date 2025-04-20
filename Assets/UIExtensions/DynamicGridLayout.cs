using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGridLayout : LayoutGroup
{
    public enum LayoutDirection
    {
        Vertical,
        Horizontal
    }
    public LayoutDirection layoutDirection;

    public float width { get { return rectTransform.rect.width - padding.left - padding.right; } }
    public float height { get { return rectTransform.rect.height - padding.top - padding.bottom; } }

    public bool IsVertical { get { return layoutDirection == LayoutDirection.Vertical; } }
    public bool IsHorizontal { get { return layoutDirection == LayoutDirection.Horizontal; } }

    public int Columns { get { return Mathf.Max(1, IsVertical ? Mathf.FloorToInt(width / (itemSize.x + spacingMin)) : Mathf.CeilToInt(rectChildren.Count / (float)Rows)); } }
    public int Rows { get { return Mathf.Max(1, IsHorizontal ? Mathf.FloorToInt(height / (itemSize.y + spacingMin)) : Mathf.CeilToInt(rectChildren.Count / (float)Columns)); } }

    public Vector2 itemSize = Vector2.one * 100f;
    public float spacingMin = 5f;
    public bool Expand;

    public override float minWidth { get { return 0; } }
    public override float minHeight { get { return 0; } }

    public override float preferredWidth { get { return Columns * (Spacing.x * itemSize.x) + padding.right + padding.left; } }
    public override float preferredHeight { get { return Rows * (Spacing.y + itemSize.y) + padding.top + padding.bottom; } }

    private Vector2 Spacing
    {
        get
        {
            var spacingX = Expand && IsVertical ? ((width / Columns) - itemSize.x) / (Columns - 1) * Columns : spacingMin;
            var spacingY = Expand && IsHorizontal ? ((height / Rows) - itemSize.y) / (Rows - 1) * Rows : spacingMin;
            return new Vector2(spacingX, spacingY);
        }
    }
    public override void CalculateLayoutInputHorizontal()
    {
        //stopped here: override existing code to create grid layout with fixed row or column count and aspectRatio
        base.CalculateLayoutInputHorizontal();
        if (rectChildren.Count == 0) return;
        var spacing = Spacing;
        var columns = Columns;
        var rows = Rows;

        var xOffset = padding.left + (width - columns * itemSize.x - (columns - 1) * spacing.x) / 2f;
        var yOffset = padding.top + (height - rows * itemSize.y - (rows - 1) * spacing.y) / 2f;
        for (var i = 0; i < rectChildren.Count; i++)
        {
            var item = rectChildren[i];
            var xIndex = layoutDirection == LayoutDirection.Vertical ? (i % columns) : Mathf.Floor(i / rows);
            var yIndex = layoutDirection == LayoutDirection.Vertical ? Mathf.Floor(i / columns) : (i % rows);
            var xPos = xIndex * (itemSize.x + spacing.x);
            var yPos = yIndex * (itemSize.y + spacing.y);
            SetChildAlongAxis(item, 0, xPos + xOffset, itemSize.x);
            SetChildAlongAxis(item, 1, yPos + yOffset, itemSize.y);
        }
    }


    public override void CalculateLayoutInputVertical()
    {

    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }
}

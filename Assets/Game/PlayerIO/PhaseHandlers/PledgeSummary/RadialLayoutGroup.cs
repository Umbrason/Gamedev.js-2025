using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RadialLayoutGroup : LayoutGroup
{
    private RectTransform m_rectTransform;
    private RectTransform rectTransform { get => m_rectTransform == null ? m_rectTransform = GetComponent<RectTransform>() : m_rectTransform; }
    private float Diameter { get => Mathf.Min(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y); }

    [Range(0, 360), SerializeField]
    private float startAngle;
    [Range(0, 360), SerializeField]
    private float arcLength;

    public float StartAngle
    {
        get => startAngle;
        set
        {
            startAngle = value;
            SetLayoutHorizontal();
        }
    }
    public float ArcLength
    {
        get => arcLength;
        set
        {
            arcLength = value;
            SetLayoutHorizontal();
        }
    }

    [SerializeField] private float holeDiameter;

    private float RadiansStartAngle => startAngle / 180f * Mathf.PI;
    private float RadiansArcLength => arcLength / 180f * Mathf.PI;

    [SerializeField] private bool ForceExpand;
    private enum Alignment
    {
        Center, Clockwise, CounterClockwise
    }
    [SerializeField] private Alignment alignment;
    [SerializeField] private float spacing;

    public override void SetLayoutHorizontal()
    {
        var enabledChildren = this.rectChildren;
        var innerRadius = holeDiameter / 2f;
        var outerRadius = Diameter / 2f;
        var centerRadius = (innerRadius + outerRadius) / 2f;

        var childSize = outerRadius - innerRadius;
        var angleStep = ForceExpand ? RadiansArcLength / (rectChildren.Count - 1) : (childSize + spacing) / centerRadius;
        angleStep *= alignment == Alignment.CounterClockwise ? -1 : 1;

        var alphaOffset = alignment switch
        {
            Alignment.Center => (RadiansArcLength - (angleStep * (rectChildren.Count - 1))) / 2f,
            Alignment.Clockwise => 0,
            Alignment.CounterClockwise => RadiansArcLength,
            _ => 0
        };
        alphaOffset += RadiansStartAngle;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            var childRectTransform = enabledChildren[i].GetComponent<RectTransform>();
            if (!childRectTransform)
            {
                Debug.LogError($"setting child's layout failed: No RectTransform present on {enabledChildren[i]}!", enabledChildren[i]);
                return;
            }
            var scale = Vector2.one * childSize;
            childRectTransform.sizeDelta = scale;
            var alpha = angleStep * i + alphaOffset;
            var vecAlpha = new Vector2(Mathf.Sin(alpha), Mathf.Cos(alpha));
            var offset = (rectTransform.pivot - Vector2.one * .5f) * -rectTransform.sizeDelta + vecAlpha * centerRadius;
            childRectTransform.localPosition = offset;
        }
    }

    public override void SetLayoutVertical() => SetLayoutHorizontal();
    /* protected override void OnRectTransformDimensionsChange() => Base.OnRectTransformDimensionsChangeSetLayoutHorizontal(); */
    public override void CalculateLayoutInputVertical() => CalculateLayoutInputHorizontal();

#if UNITY_EDITOR
    /* private void OnValidate() => SetLayoutHorizontal(); */

    private void OnDrawGizmosSelected()
    {
        var screenSize = rectTransform.TransformVector(rectTransform.sizeDelta);
        var screenRadius = Mathf.Min(screenSize.x, screenSize.y) / 2f;
        var screenHoleSize = rectTransform.TransformVector(new(holeDiameter / 2f, holeDiameter / 2f));
        var screenHoleRadius = Mathf.Min(screenHoleSize.x, screenHoleSize.y);
        /* UnityEditor.Handles.DrawWireDisc(rectTransform.position, rectTransform.forward, screenRadius);
        UnityEditor.Handles.DrawWireDisc(rectTransform.position, rectTransform.forward, screenHoleRadius); */

        var minAlpha = RadiansStartAngle;
        var maxAlpha = RadiansStartAngle + RadiansArcLength;
        var arcAlpha = RadiansStartAngle + RadiansArcLength;

        var minAngleDirection = new Vector2(Mathf.Sin(minAlpha), Mathf.Cos(minAlpha));
        var maxAngleDirection = new Vector2(Mathf.Sin(maxAlpha), Mathf.Cos(maxAlpha));
        var arcCenterDirection = new Vector2(Mathf.Sin(arcAlpha), Mathf.Cos(arcAlpha));

        UnityEditor.Handles.DrawWireArc(rectTransform.position, rectTransform.forward, arcCenterDirection, arcLength, screenRadius);
        UnityEditor.Handles.DrawWireArc(rectTransform.position, rectTransform.forward, arcCenterDirection, arcLength, screenHoleRadius);
        UnityEditor.Handles.DrawLine((Vector2)rectTransform.position + minAngleDirection * screenRadius, (Vector2)rectTransform.position + minAngleDirection * screenHoleRadius);
        UnityEditor.Handles.DrawLine((Vector2)rectTransform.position + maxAngleDirection * screenRadius, (Vector2)rectTransform.position + maxAngleDirection * screenHoleRadius);
    }

#endif
}

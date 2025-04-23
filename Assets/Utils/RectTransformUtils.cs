using System.Linq;
using UnityEngine;

public static class RectTransformUtils
{
    private static Vector3[] WorldCorners = new Vector3[4]; //BL->TL->TR->BR
    public static Vector2 ScreenPosition(this RectTransform transform)
    {
        var rootCanvas = transform.GetComponentsInParent<Canvas>().LastOrDefault()?.GetComponent<RectTransform>();
        if (rootCanvas == null)
        {
            Debug.LogError("Cant find screen position of rect transform outside of a canvas environment");
            return default;
        }
        rootCanvas.GetWorldCorners(WorldCorners);
        var min = WorldCorners[0];

        var up = WorldCorners[1] - WorldCorners[0];
        up /= up.sqrMagnitude;

        var right = WorldCorners[3] - WorldCorners[0];
        right /= right.sqrMagnitude;


        transform.GetWorldCorners(WorldCorners);
        var local = (WorldCorners[0] + WorldCorners[2]) / 2f - min;

        var screenSpace = new Vector2(Vector3.Dot(local, right), Vector3.Dot(local, up));
        return screenSpace;
    }
}
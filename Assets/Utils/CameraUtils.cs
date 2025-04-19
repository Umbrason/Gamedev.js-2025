using UnityEngine;

public static class CameraUtils
{
    public static Vector3 ScreenPointToXZIntersection(this Camera camera, Vector2 screenPoint)
    {
        var ray = camera.ScreenPointToRay(screenPoint);
        var t = ray.origin.y / -ray.direction.y;
        return ray.GetPoint(t);
    }
}
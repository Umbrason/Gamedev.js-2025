using UnityEngine;

public struct HexPosition
{
    public HexPosition(int q, int r) : this(q, r, -q - r) { }
    public HexPosition(int q, int r, int s)
    {
        Q = q;
        R = r;
        if (S != -Q - R) throw new System.Exception("Invalid HexPosition");
    }

    public int Q { get; set; }
    public int R { get; set; }
    public int S => -Q - R;

    public readonly Vector3 WorldPositionCenter => (HexOrientation.Active * this)._x0y();

    public static HexPosition operator -(HexPosition a)
        => new(-a.Q, -a.R);
    public static HexPosition operator +(HexPosition a, HexPosition b)
        => new(a.Q - b.Q, a.R - b.R);
    public static HexPosition operator *(HexPosition a, int skalar)
        => new(a.Q * skalar, a.R * skalar);
}
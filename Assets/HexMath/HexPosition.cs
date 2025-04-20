
using System.Collections.Generic;
using UnityEngine;

public struct HexPosition : ISerializable<HexPosition>
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
        => new(a.Q + b.Q, a.R + b.R);
    public static HexPosition operator -(HexPosition a, HexPosition b)
    => a + (-b);
    public static HexPosition operator *(HexPosition a, int skalar)
        => new(a.Q * skalar, a.R * skalar);
    public static HexPosition operator *(int skalar, HexPosition a)
    => new(a.Q * skalar, a.R * skalar);
}

public static class HexPositionExtensions
{
    private static readonly HexPosition[] directions = new HexPosition[6]
    {
        new HexPosition(1, 0, -1), new HexPosition(1, -1, 0), new HexPosition(0, -1, 1),
        new HexPosition(-1, 0, 1), new HexPosition(-1, 1, 0), new HexPosition(0, 1, -1),
    };
    public static List<HexPosition> GetSurrounding(this HexPosition origin)
    {
        List<HexPosition> list = new List<HexPosition>();
        for (int i = 0; i < directions.Length; i++) list.Add(origin + directions[i]);
        return list;
    }
}
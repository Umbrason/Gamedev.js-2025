using UnityEngine;

public struct HexOrientation
{
    private readonly float f0, f1, f2, f3;
    private readonly float b0, b1, b2, b3;

    const float Sqrt3 = 1.73205080757f;
    const float Half_Sqrt3 = 0.86602540378f;
    const float Third_Sqrt3 = 0.57735026919f;
    public static readonly HexOrientation Flat = new(Sqrt3, Half_Sqrt3, 0f, 1.5f, Third_Sqrt3, -.3333333333f, 0f, .6666666666f);
    public static readonly HexOrientation Pointy = new(1.5f, 0f, Half_Sqrt3, Sqrt3, .6666666666f, 0f, -.3333333333f, Third_Sqrt3);
    public static HexOrientation Active => Pointy;

    public HexOrientation(float f0, float f1, float f2, float f3, float b0, float b1, float b2, float b3)
    {
        //2x2 matrix
        this.f0 = f0;
        this.f1 = f1;
        this.f2 = f2;
        this.f3 = f3;
        //inverse
        this.b0 = b0;
        this.b1 = b1;
        this.b2 = b2;
        this.b3 = b3;
    }

    public static Vector2 operator *(HexOrientation o, HexPosition p)
    {
        float x = o.f0 * p.Q + o.f1 * p.R;
        float y = o.f2 * p.Q + o.f3 * p.R;
        return new Vector2(y, x);
    }

    public static HexPosition operator *(HexOrientation o, Vector3 worldPosition) => o * new Vector2(worldPosition.x, worldPosition.z);
    public static HexPosition operator *(HexOrientation o, Vector2 worldPosition)
    {
        float frac_q = o.b0 * worldPosition.y + o.b1 * worldPosition.x;
        float frac_r = o.b2 * worldPosition.y + o.b3 * worldPosition.x;
        float frac_s = -frac_q - frac_r;

        var q = Mathf.RoundToInt(frac_q);
        var r = Mathf.RoundToInt(frac_r);
        var s = Mathf.RoundToInt(frac_s);

        var q_diff = Mathf.Abs(q - frac_q);
        var r_diff = Mathf.Abs(r - frac_r);
        var s_diff = Mathf.Abs(s - frac_s);

        if (q_diff > r_diff && q_diff > s_diff) q = -r - s;
        else if (r_diff > s_diff) r = -q - s;
        else s = -q - r;
        return new HexPosition(q, r, s);
    }
}
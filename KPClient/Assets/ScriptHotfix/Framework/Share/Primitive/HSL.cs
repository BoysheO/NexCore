namespace Primitive{

public readonly struct HSL
{
    /// <summary>
    /// hue,[0,360)
    /// </summary>
    public readonly float H;

    public readonly float S;
    public readonly float L;
    public readonly float A;

    public HSL(float h, float s, float l, float alpha)
    {
        H = h;
        S = s;
        L = l;
        A = alpha;
    }

    public HSL AddHSL(float h, float s, float l)
    {
        return new((h + H) % 360, Clamp(s + S, 0, 1), Clamp(l + L, 0, 1), A);
    }

    public ColorF ToColor()
    {
        float r, g, b;
        if (S == 0)
        {
            r = g = b = L;
        }
        else
        {
            var q = L < 0.5 ? L * (1 + S) : L + S - L * S;
            var p = 2 * L - q;
            r = Hue2RGB(p, q, H + 120);
            g = Hue2RGB(p, q, H);
            b = Hue2RGB(p, q, H - 120);
        }

        return new ColorF(r, g, b, A);
    }

    private float Hue2RGB(float p, float q, float t)
    {
        if (t < 0) t += 360;
        if (t > 360) t -= 360;
        if (t < 60) return p + (q - p) * t / 60;
        if (t < 180) return q;
        if (t < 240) return p + (q - p) * (240 - t) / 60;
        return p;
    }

    private float Clamp(float value, float min, float max)
    {
        return value < min ? min : value > max ? max : value;
    }
}}
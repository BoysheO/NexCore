namespace Primitive{

public readonly struct HSV
{
    /// <summary>
    /// hue,[0,360)
    /// </summary>
    public readonly float H;
    /// <summary>
    /// saturation,[0,1]
    /// </summary>
    public readonly float S;
    /// <summary>
    /// value,[0,1]
    /// </summary>
    public readonly float V;
    /// <summary>
    /// alpha,[0,1]
    /// </summary>
    public readonly float A;

    public HSV(float h, float s, float v, float alpha)
    {
        H = h;
        S = s;
        V = v;
        A = alpha;
    }

    public HSV AddHSV(float h, float s, float v)
    {
        return new((h + H) % 360, Clamp(s + S, 0, 1), Clamp(v + V, 0, 1), A);
    }

    public ColorF ToColor()
    {
        float r, g, b;
        if (S == 0)
        {
            r = g = b = V;
        }
        else
        {
            var i = (int) (H / 60) % 6;
            var f = H / 60 - i;
            var p = V * (1 - S);
            var q = V * (1 - f * S);
            var t = V * (1 - (1 - f) * S);
            switch (i)
            {
                case 0:
                    r = V;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = V;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = V;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = V;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = V;
                    break;
                default:
                    r = V;
                    g = p;
                    b = q;
                    break;
            }
        }

        return new ColorF(r, g, b, A);
    }

    private float Clamp(float value, float min, float max)
    {
        return value < min ? min : value > max ? max : value;
    }
}}
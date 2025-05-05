using System;

namespace Primitive{

public readonly struct ColorF
{
    public readonly float R;
    public readonly float G;
    public readonly float B;
    public readonly float A;

    public ColorF(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public ColorF AddRGBNoClamp(float r, float g, float b)
    {
        return new(R + r, G + g, B + b, A);
    }

    public ColorF AddRGB(float r, float g, float b)
    {
        return new(Clamp(r + R, 0, 1), Clamp(g + G, 0, 1), Clamp(b + B, 0, 1), A);
    }

    private float Clamp(float value, float min, float max)
    {
        return value < min ? min : value > max ? max : value;
    }

    public HSL ToHSL()
    {
        // ReSharper disable CompareOfFloatsByEqualityOperator
        var max = Math.Max(R, Math.Max(G, B));
        var min = Math.Min(R, Math.Min(G, B));
        var delta = max - min;
        //hue,[0,360)
        var h = 0f;
        if (delta == 0)
        {
            h = 0;
        }
        else if (max == R && G >= B)
        {
            h = 60 * (G - B) / delta;
        }
        else if (max == R && G < B)
        {
            h = 60 * (G - B) / delta + 360;
        }
        else if (max == G)
        {
            h = 60 * (B - R) / delta + 120;
        }
        else if (max == B)
        {
            h = 60 * (R - G) / delta + 240;
        }

        //lightness,[0,1]
        var l = (max + min) / 2;

        //saturation,[0,1]
        var s = 0f;
        if (l == 0 || max == min)
        {
            s = 0;
        }
        else if (l < 0 || l <= 0.5f)
        {
            s = delta / (2 * l);
        }
        else if (l > 0.5f)
        {
            s = delta / (2 - 2 * l);
        }

        return new HSL(h, s, l, A);
        // ReSharper restore CompareOfFloatsByEqualityOperator
    }

    public HSV ToHSV()
    {
        float h = 0, s, v;
        //cover color to hsv
        var max = Math.Max(R, Math.Max(G, B));
        var min = Math.Min(R, Math.Min(G, B));
        var delta = max - min;
        //hue,[0,360)
        if (delta == 0)
        {
            h = 0;
        }
        else if (max == R && G >= B)
        {
            h = 60 * (G - B) / delta;
        }
        else if (max == R && G < B)
        {
            h = 60 * (G - B) / delta + 360;
        }
        else if (max == G)
        {
            h = 60 * (B - R) / delta + 120;
        }
        else if (max == B)
        {
            h = 60 * (R - G) / delta + 240;
        }

        //saturation,[0,1]
        s = max == 0 ? 0 : delta / max;

        //value,[0,1]
        v = max;

        return new HSV(h, s, v, A);
    }

    /// <summary>
    /// 在假定本颜色不是HDR颜色的前提下，直接添加HDR系数Intensity
    /// </summary>
    public ColorF HDRIntensity(float i)
    {
        var fac = (float) Math.Pow(2, i);
        return new ColorF(fac * R, fac * G, fac * B, A);
    }

    /// <summary>
    /// 在假定本颜色是HDR颜色的前提下，提取原本的颜色和系数
    /// *未测试，不知此函数正确性
    /// </summary>
    public ColorRGBA DecomposeHdrColor(out float exposure)
    {
        float maxColorComponent;
        {
            maxColorComponent = R;
            if (G > maxColorComponent) maxColorComponent = G;
            if (B > maxColorComponent) maxColorComponent = B;
        }

        // replicate Photoshops's decomposition behaviour
        if (maxColorComponent == 0f || maxColorComponent <= 1f && maxColorComponent >= 1 / 255f)
        {
            exposure = 0f;
            var r = (byte) Math.Round(R * 255f);
            var g = (byte) Math.Round(G * 255f);
            var b = (byte) Math.Round(B * 255f);
            var a = (byte) Math.Round(A * 255f);
            return new ColorRGBA(r, g, b, a);
        }
        else
        {
            const byte k_MaxByteForOverexposedColor = 191;
            // calibrate exposure to the max float color component
            var scaleFactor = k_MaxByteForOverexposedColor / maxColorComponent;
            exposure = (float) (Math.Log(255f / scaleFactor) / Math.Log(2f));
            // maintain maximal integrity of byte values to prevent off-by-one errors when scaling up a color one component at a time
            var r = Math.Min(k_MaxByteForOverexposedColor,
                (byte) Math.Ceiling(scaleFactor * R));
            var g = Math.Min(k_MaxByteForOverexposedColor,
                (byte) Math.Ceiling(scaleFactor * G));
            var b = Math.Min(k_MaxByteForOverexposedColor,
                (byte) Math.Ceiling(scaleFactor * B));
            var a = (byte) Math.Round(A * 255f);
            return new ColorRGBA(r, g, b, a);
        }
    }
}}
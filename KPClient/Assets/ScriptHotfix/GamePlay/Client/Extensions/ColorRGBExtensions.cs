using Primitive;
using UnityEngine;

namespace Hotfix.Extensions{

public static class ColorRGBExtensions
{
    public static Color ToUnityColor(this ColorRGBA colorRgba)
    {
        return new Color(colorRgba.R * 1f / byte.MaxValue, colorRgba.G * 1f / byte.MaxValue,
            colorRgba.B * 1f / byte.MaxValue, colorRgba.A * 1f / byte.MaxValue);
    }
}}
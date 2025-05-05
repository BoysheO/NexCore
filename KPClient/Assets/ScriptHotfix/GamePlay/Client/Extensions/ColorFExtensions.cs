using Primitive;
using UnityEngine;

namespace Hotfix.Extensions{

public static class ColorFExtensions
{
    public static Color ToUnityColor(this ColorF colorF)
    {
        return new Color(colorF.R, colorF.G, colorF.B, colorF.A);
    }
}}
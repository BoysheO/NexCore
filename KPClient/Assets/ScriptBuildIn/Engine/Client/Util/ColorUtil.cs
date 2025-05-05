using System;
using BoysheO.Util;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.Util
{
    public static class ColorUtil
    {
        public static Color32 ToColor32(ReadOnlySpan<char> colorHexString, byte defaultAlpha = default)
        {
            //tips:现在有新的API了，可以用ColorUtility处理
            if (colorHexString.Length < 6)
                throw new ArgumentOutOfRangeException($"{nameof(colorHexString)} is too short.");
            var len = colorHexString.Length;
            if (colorHexString[0] == '#') colorHexString = colorHexString.Slice(1);

            var r = ByteUtil.HexCharToByte(colorHexString[0], colorHexString[1]);
            var g = ByteUtil.HexCharToByte(colorHexString[2], colorHexString[3]);
            var b = ByteUtil.HexCharToByte(colorHexString[4], colorHexString[5]);
            if (colorHexString.Length <= 6) return new Color32(r, g, b, defaultAlpha);
            var a = ByteUtil.HexCharToByte(colorHexString[6], colorHexString[7]);
            return new Color32(r, g, b, a);
        }

        public static Color32 ToColor32(string colorHexString, byte defaultAlpha = default)
        {
            return ToColor32(colorHexString.AsSpan(),defaultAlpha);
        }
        
        /// <summary>
        ///  根据某公式获取两色对比度
        /// 黑色#000000白色#ffffff对比度为21
        /// </summary>
        public static float GetD(Color a, Color b)
        {
            return (getL(a) + 0.05f) / (getL(b) + 0.05f);
        
            float getL(Color c)
            {
                var r = process(c.r);
                var g = process(c.g);
                var b = process(c.b);
                return 0.2126f * r + 0.7152f * g + 0.0722f * b;
            }

            float process(float v)
            {
                v = v.Min(0);
                if (v < 0.03928f) return v / 12.92f;
                return Mathf.Pow((v + 0.055f) / 1.055f, 2.4f);
            }
        }
    }
}
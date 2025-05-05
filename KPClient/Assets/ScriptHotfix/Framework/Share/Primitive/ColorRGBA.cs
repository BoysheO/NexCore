using System.Globalization;
using System;

namespace Primitive
{
    /// <summary>
    /// 表示32位颜色（Color32），包含红、绿、蓝、透明度（RGBA）。
    /// 使用 <see cref="ToRGBAHex"/> 获取 "#RRGGBBAA" 字串。
    /// 使用 <see cref="ToRGBHex"/> 获取 "#RRGGBB" 字串。
    /// </summary>
    public readonly struct ColorRGBA : IEquatable<ColorRGBA>
    {
        public readonly byte R;
        public readonly byte G;
        public readonly byte B;
        public readonly byte A;
        private const float ByteToFloat = 1f / 255f;

        public uint RGBA => (uint)(R << 24 | G << 16 | B << 8 | A);

        public uint RGB => (uint)(R << 16 | G << 8 | B);

        public ColorRGBA(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = byte.MaxValue;
        }

        public ColorRGBA(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public ColorRGBA(int rgba) : this(checked((uint)rgba))
        {
            //do nothing
        }

        public ColorRGBA(uint rgba) : this(
            (byte)((rgba >> 24) & 0xFF),
            (byte)((rgba >> 16) & 0xFF),
            (byte)((rgba >> 8) & 0xFF),
            (byte)(rgba & 0xFF))
        {
            //do nothing
        }

        public ColorF ToColorF() => 
            new(R * ByteToFloat, G * ByteToFloat, B * ByteToFloat, A * ByteToFloat);

        public string ToRGBAHex()
        {
            return string.Create(9, this, static (span, color) =>
            {
                span[0] = '#';
                color.R.TryFormat(span.Slice(1, 2), out _, "X2");
                color.G.TryFormat(span.Slice(3, 2), out _, "X2");
                color.B.TryFormat(span.Slice(5, 2), out _, "X2");
                color.A.TryFormat(span.Slice(7, 2), out _, "X2");
            });
        }

        public string ToRGBHex() => string.Create(7, this, static (span, color) =>
        {
            span[0] = '#';
            color.R.TryFormat(span.Slice(1, 2), out _, "X2");
            color.G.TryFormat(span.Slice(3, 2), out _, "X2");
            color.B.TryFormat(span.Slice(5, 2), out _, "X2");
        });

        /// <summary>
        /// 从十六进制字符串创建颜色。例如 "#FFAABB" 或 "#FFAABBCC"。
        /// 支持6位（不含Alpha）或8位（含Alpha）格式。
        /// </summary>
        public static ColorRGBA FromHex(string hex)
        {
            ReadOnlySpan<char> span = hex.AsSpan();
            if (span[0] == '#') span = span.Slice(1);

            if (span.Length is not 6 and not 8)
                throw new FormatException("Invalid hex format");

            if (!uint.TryParse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint val))
                throw new FormatException("Invalid hex number format");

            return span.Length switch
            {
                6 => new ColorRGBA((byte)(val >> 16), (byte)(val >> 8), (byte)val),
                8 => new ColorRGBA(val),
                _ => throw new FormatException("Invalid hex length")
            };
        }

        public override string ToString() => ToRGBAHex();

        public override bool Equals(object obj) =>
            obj is ColorRGBA other && this.RGBA == other.RGBA;

        public override int GetHashCode() => RGBA.GetHashCode();

        public static bool operator ==(ColorRGBA a, ColorRGBA b) => a.RGBA == b.RGBA;
        public static bool operator !=(ColorRGBA a, ColorRGBA b) => a.RGBA != b.RGBA;

        public bool Equals(ColorRGBA other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }
    }
}
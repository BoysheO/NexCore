#nullable enable
namespace Primitive
{
    public readonly struct ULongXY
    {
        public readonly ulong XY;

        public int X => (int) (XY >> 32);
        public int Y => (int) (XY & 0xffffffff);

        public ULongXY(int x, int y)
        {
            XY = ((ulong) x << 32) | (uint) y;
        }

        public ULongXY(ulong xy)
        {
            XY = xy;
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public bool Equals(ULongXY other)
        {
            return XY == other.XY;
        }

        public override bool Equals(object? obj)
        {
            return obj is ULongXY other && Equals(other);
        }

        public override int GetHashCode()
        {
            return XY.GetHashCode();
        }


        public static bool operator ==(ULongXY a, ULongXY b)
        {
            return a.XY == b.XY;
        }

        public static bool operator !=(ULongXY a, ULongXY b)
        {
            return !(a == b);
        }
    }
}
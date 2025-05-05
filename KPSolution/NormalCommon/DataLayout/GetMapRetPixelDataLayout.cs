using System;
using System.Runtime.InteropServices;
using BoysheO.Buffers;

namespace NormalCommon;
[StructLayout(LayoutKind.Sequential)]
public struct GetMapRetPixelDataLayout
{
    public int X;
    public int Y;
    public byte Color;
    public uint DurationBetweenStandardTimeSec;

    /// <summary>
    /// 在不能使用system.memory情况下的替代
    /// </summary>
    public static void FromBytes(PooledListBuffer<byte> bytes,GetMapRetPixelDataLayout[] buffer)
    {
        var s = MemoryMarshal.Cast<byte, GetMapRetPixelDataLayout>(bytes.Span);
        if (buffer.Length < s.Length) throw new Exception("buffer not large enough");
        s.CopyTo(buffer);
    }
}
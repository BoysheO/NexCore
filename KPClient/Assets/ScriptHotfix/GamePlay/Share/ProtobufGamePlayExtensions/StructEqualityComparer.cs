using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;

namespace CommonCode.ProtobufGamePlayExtensions{

/// <summary>
/// 在内置比对函数上封装一层判断各自null情况
/// </summary>
public sealed class StructEqualityComparer : IEqualityComparer<Struct>
{
    public static readonly StructEqualityComparer Instance = new();

    public bool Equals(Struct? x, Struct? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        return x.Equals(y);
    }

    public int GetHashCode(Struct obj)
    {
        return obj.GetHashCode();
    }
}}
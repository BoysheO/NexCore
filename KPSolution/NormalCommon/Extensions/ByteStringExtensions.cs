using Google.Protobuf;

namespace BoysheO.Buffers.Extensions;

public static class ByteStringExtensions
{
    public static PooledListBuffer<byte> GetBytes(this ByteString byteString)
    {
        var buffer = PooledListBuffer<byte>.Rent();
        var span = byteString.Span;
        var aSpan = buffer.GetSpanAdding(span.Length);
        span.CopyTo(aSpan);
        return buffer;
    }
}
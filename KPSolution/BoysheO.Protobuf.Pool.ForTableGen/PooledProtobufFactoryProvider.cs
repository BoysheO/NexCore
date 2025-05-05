using System;
using Google.Protobuf;

namespace BoysheO.Protobuf.Pooled
{
    public sealed class PooledProtobufFactoryProvider : IProtobufFactoryProvider
    {
        public Func<T> GetFactory<T>() where T : IBufferMessage, IMessage<T>, new()
        {
            return PbPool<T>.Rent;
        }
    }
}
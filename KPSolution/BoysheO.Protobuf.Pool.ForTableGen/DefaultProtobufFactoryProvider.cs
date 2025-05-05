using System;
using Google.Protobuf;

namespace BoysheO.Protobuf.Pooled
{
    public sealed class DefaultProtobufFactoryProvider : IProtobufFactoryProvider
    {
        public Func<T> GetFactory<T>() where T : IBufferMessage, IMessage<T>, new()
        {
            return () => new T();
        }
    }
}
using Google.Protobuf;
using System;

namespace BoysheO.Protobuf.Pooled
{
    public interface IProtobufFactoryProvider
    {
        Func<T> GetFactory<T>() where T : IBufferMessage, IMessage<T>, new();
    }
}
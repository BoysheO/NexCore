using System;
using Google.Protobuf;

namespace BoysheO.Protobuf.Pooled
{
    public interface IProtobufFactoryProvider
    {
        Func<T> GetFactory<T>() where T : IBufferMessage, IMessage<T>, new();
    }
}
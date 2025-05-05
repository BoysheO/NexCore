using System;
using System.Threading;
using Google.Protobuf;

namespace BoysheO.Protobuf.Pooled
{
    public static class ProtobufFactoryContext
    {
        private static IProtobufFactoryProvider _protobufFactoryProvider = new DefaultProtobufFactoryProvider();
        private static int _isNoChance;

        public static void SetFactoryProvider(IProtobufFactoryProvider factoryProvider)
        {
            var isNoChance = Interlocked.Exchange(ref _isNoChance, 1);
            if (isNoChance > 0) throw new Exception("you have lose the chance to set up factory provider");
            _protobufFactoryProvider = factoryProvider;
        }

        public static void UsePooledFactoryProvider()
        {
            SetFactoryProvider(new PooledProtobufFactoryProvider());
        }

        public static Func<T> GetFactory<T>()
            where T : IBufferMessage, IMessage<T>, new()
        {
            Interlocked.Exchange(ref _isNoChance, 1);
            return _protobufFactoryProvider.GetFactory<T>();
        }
    }
}
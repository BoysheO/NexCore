using System;
using System.Collections.Concurrent;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;


namespace BoysheO.Protobuf.Pooled
{
    public sealed class PbPool<T>
        where T : IBufferMessage, IMessage<T>, new()
    {
        private static readonly WeakReference<ConcurrentBag<T>> _pool =
            new WeakReference<ConcurrentBag<T>>(new ConcurrentBag<T>());

        private static readonly T _empty = new T();

        public static T Rent()
        {
            if (!_pool.TryGetTarget(out var bag) || !bag.TryTake(out var ins))
            {
                return new T();
            }

            return ins;
        }

        public static void Return(T ins)
        {
            if (ins == null) throw new ArgumentNullException(nameof(ins));
            ins.MergeFrom(ByteString.Empty);
            if (!_pool.TryGetTarget(out var bag))
            {
                bag = new ConcurrentBag<T>();
                _pool.SetTarget(bag);
            }

            bag.Add(ins);
        }
    }
}
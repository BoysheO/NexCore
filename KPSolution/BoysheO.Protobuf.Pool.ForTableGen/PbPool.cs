using System;
using System.Collections.Concurrent;
using Google.Protobuf;

namespace BoysheO.Protobuf.Pooled
{
    public sealed class PbPool<T>
        where T : IBufferMessage, IMessage<T>, new()
    {
        private static readonly WeakReference<ConcurrentBag<T>> _pool =
            new WeakReference<ConcurrentBag<T>>(new ConcurrentBag<T>());

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
            if (!_pool.TryGetTarget(out var bag))
            {
                bag = new ConcurrentBag<T>();
                _pool.SetTarget(bag);
            }

            bag.Add(ins);
        }
    }
}
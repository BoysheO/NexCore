using System;
using UniRx;
using UnityEngine.Scripting;

namespace NexCore.Extensions
{
    [Preserve]
    public static class RxExtensions
    {
        public static IObservable<ValueTuple> ToValueTupleObservable(this IObservable<Unit> observable)
        {
            return observable.Select<Unit, ValueTuple>(_ => default);
        }
    }
}
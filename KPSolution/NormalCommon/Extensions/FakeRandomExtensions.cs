using System;
using System.Collections.Generic;

namespace BoysheO.Buffers.Extensions;

public static class FakeRandomExtensions
{
    public static T DrawByTime<T>(this IReadOnlyList<T> lst)
    {
        if (lst == null) throw new ArgumentNullException(nameof(lst),$"arg lst<{typeof(T).Name}> is null");
        if (lst.Count == 0) throw new ArgumentOutOfRangeException(nameof(lst),$"lst<{typeof(T).Name}> has items not enough ");
        if (lst.Count == 1) return lst[0];
        var time = DateTimeOffset.Now.Second;
        return lst[time % lst.Count];
    }
}
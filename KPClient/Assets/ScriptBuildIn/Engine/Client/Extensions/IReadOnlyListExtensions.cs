using System.Collections.Generic;

namespace NexCore.Extensions
{

    public static class IReadOnlyListExtensions
    {
        public static int IndexOf<T>(this IReadOnlyList<T> lst, T t, EqualityComparer<T> equalityComparer)
        {
            for (var i = 0; i < lst.Count; i++)
            {
                if (equalityComparer.Equals(t, lst[i])) return i;
            }

            return -1;
        }
    }
}
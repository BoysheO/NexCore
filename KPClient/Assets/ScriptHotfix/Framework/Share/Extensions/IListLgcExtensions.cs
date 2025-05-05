using System;
using System.Buffers;
using System.Collections.Generic;
using BoysheO.Collection2;

namespace HotScripts.Hotfix.SharedCode.SharedCode.Extensions
{
    public static class IListLgcExtensions
    {
        public static IList<T> AddRangeLgc<T>(this IList<T> lst1, IReadOnlyList<T> lst2)
        {
            for (var index = 0; index < lst2.Count; index++)
            {
                var x1 = lst2[index];
                lst1.Add(x1);
            }

            return lst1;
        }


        public static bool IsAnySameElement<T>(this IReadOnlyList<T> lst)
        {
            using var buffer = VList<T>.Rent();
            for (var index = 0; index < lst.Count; index++)
            {
                var x1 = lst[index];
                if (buffer.Contains(x1)) return true;
                buffer.Add(x1);
            }

            return false;
        }

        public static T LastLgc<T>(this IReadOnlyList<T> lst)
        {
            if (lst.Count == 0) throw new ArgumentException(nameof(lst));
            return lst[^1];
        }
        
        public static T? LastOrDefaultLgc<T>(this IReadOnlyList<T> lst)
        {
            return lst.Count == 0 ? default : lst[^1];
        }
        
        public static T? FirstOrDefaultLgc<T>(this IReadOnlyList<T> lst)
        {
            return lst.Count == 0 ? default : lst[0];
        }
    }
}
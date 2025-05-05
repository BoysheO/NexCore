using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions.Unity3D.Extensions
{
    public static class ObjectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? TrimFakeNull<T>(this T? obj)
            where T : UnityEngine.Object
        {
            return obj == null ? null : obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNullOrFakeNull<T>(this T? obj)
            where T : UnityEngine.Object
        {
            if (obj == null) throw new NullReferenceException($"target is null");
            return obj;
        }
    }
}
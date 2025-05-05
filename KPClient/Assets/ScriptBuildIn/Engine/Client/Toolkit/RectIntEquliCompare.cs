using System.Collections.Generic;
using UnityEngine;

namespace Hotfix.ContentSystems.LocalDbSystem
{
    public sealed class RectIntEqualityComparer : IEqualityComparer<RectInt>
    {
        public static readonly RectIntEqualityComparer Instance = new();

        public bool Equals(RectInt x, RectInt y)
        {
            return x.width == y.width && x.height == y.height && x.xMin == y.xMin && x.yMin == y.yMin;
        }

        public int GetHashCode(RectInt obj)
        {
            unchecked
            {
                var hashCode = obj.width;
                hashCode = (hashCode * 397) ^ obj.height;
                hashCode = (hashCode * 397) ^ obj.xMin;
                hashCode = (hashCode * 397) ^ obj.yMin;
                return hashCode;
            }
        }
    }
}
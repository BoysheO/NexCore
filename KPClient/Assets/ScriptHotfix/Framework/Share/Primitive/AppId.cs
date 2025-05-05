using System;
using System.Collections.Generic;
using BoysheO.Extensions;

namespace Primitive
{
    //AppId是指某个具体应用的名称，例如com.boysheo.exampleapp
    public readonly struct AppId : IEquatable<AppId>
    {
        private sealed class IdEqualityComparer : IEqualityComparer<AppId>
        {
            public bool Equals(AppId x, AppId y)
            {
                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(AppId obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        public static IEqualityComparer<AppId> IdComparer { get; } = new IdEqualityComparer();

        public readonly string Id;

        public AppId(string id)
        {
            id.ThrowIfNullOrWhiteSpace();
            Id = id;
        }

        public bool IsInvalid => Id != null;

        public override string ToString()
        {
            return Id;
        }

        public bool Equals(AppId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object? obj)
        {
            return obj is AppId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(AppId left, AppId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AppId left, AppId right)
        {
            return !left.Equals(right);
        }
    }
}
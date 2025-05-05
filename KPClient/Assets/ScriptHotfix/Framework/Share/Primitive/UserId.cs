using System;
using System.Collections.Generic;

namespace Primitive
{
    //UserId和AccountId是同一语义
    public readonly struct UserId : IEquatable<UserId>
    {
        private sealed class IdEqualityComparer : IEqualityComparer<UserId>
        {
            public bool Equals(UserId x, UserId y)
            {
                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(UserId obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        public static IEqualityComparer<UserId> IdComparer { get; } = new IdEqualityComparer();

        public readonly Guid Id;

        public UserId(string guid) : this(Guid.Parse(guid))
        {
        }

        public UserId(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id), "id can not be empty");
            Id = id;
        }

        public bool IsInvalid => Id != Guid.Empty;

        public override string ToString()
        {
            return Id.ToString("N");
        }

        public bool Equals(UserId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object? obj)
        {
            return obj is UserId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(UserId left, UserId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UserId left, UserId right)
        {
            return !left.Equals(right);
        }
    }
}
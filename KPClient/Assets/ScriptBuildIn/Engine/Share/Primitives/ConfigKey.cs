using System;
using BoysheO.Extensions;

namespace ScriptEngine.BuildIn.ShareCode.Primitives
{
    //为了便于全局搜索，在使用Configuration系统时，应要求将key包装成此类型
    public readonly struct ConfigKey : IEquatable<ConfigKey>, IComparable<ConfigKey>
    {
        public readonly string Value;

        public ConfigKey(string value)
        {
            value.ThrowIfNullOrWhiteSpace();
            Value = value;
        }

        #region 功能函数

        public override bool Equals(object? obj)
        {
            return obj is ConfigKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(ConfigKey other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public int CompareTo(ConfigKey other) => string.Compare(Value, other.Value, StringComparison.Ordinal);

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(ConfigKey key) => key.Value;
        public static bool operator ==(ConfigKey left, ConfigKey right) => left.Equals(right);
        public static bool operator !=(ConfigKey left, ConfigKey right) => !left.Equals(right);

        #endregion
    }
}
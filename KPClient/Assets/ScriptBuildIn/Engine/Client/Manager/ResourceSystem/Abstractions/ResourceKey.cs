using System;

namespace Hotfix.ResourceMgr.Abstractions
{
    /// <summary>
    /// This is your customer key
    /// </summary>
    public readonly struct ResourceKey
    {
        public readonly string Value;
        public readonly Type Type;

        public ResourceKey(string path, Type type)
        {
            Value = path;
            Type = type;
        }

        public override string ToString()
        {
            return $"[{Value}:{Type.Name}]";
        }
    }
}
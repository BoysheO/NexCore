using System.Diagnostics;

namespace TableDataGenerator.Model
{
    [DebuggerDisplay("{ToDebugString()}")]
    public readonly struct ColAttributes
    {

        public readonly AttributeName Name;
        public readonly string Value;

        public ColAttributes(AttributeName name, string value)
        {
            Name = name;
            Value = value;
        }

        public string ToDebugString()
        {
            return $"[{Name.Name}:{Value}]";
        }
    }
}

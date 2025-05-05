using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableDataGenerator.Model
{
    [DebuggerDisplay("{ToDebugString()}")]
    public readonly struct AttributeName
    {
        public static readonly AttributeName FieldName = new("Name");
        public static readonly AttributeName FieldType = new("Type");
        public static readonly AttributeName Comment = new("Comment");
        public static readonly AttributeName FieldTag = new("Tag");

        public readonly string Name;

        public AttributeName(string name)
        {
            Name = name;
        }

        public string ToDebugString()
        {
            return Name;
        }
    }

    public class AttributeNameCompare : IEqualityComparer<AttributeName>, IComparer<AttributeName>
    {
        public static readonly AttributeNameCompare Instance = new AttributeNameCompare();
        public int Compare(AttributeName x, AttributeName y)
        {
            return Comparer<string>.Default.Compare(x.Name, y.Name);
        }

        public bool Equals(AttributeName x, AttributeName y)
        {
            return EqualityComparer<string>.Default.Equals(x.Name, y.Name);
        }

        public int GetHashCode([DisallowNull] AttributeName obj)
        {
            return EqualityComparer<string>.Default.GetHashCode(obj.Name);
        }
    }
}

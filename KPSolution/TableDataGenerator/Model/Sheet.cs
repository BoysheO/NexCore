using LinqForEEPlus;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableDataGenerator.Model
{
    public sealed class Sheet
    {
        public readonly string Name;
        public readonly ImmutableSortedDictionary<ColOrder, ImmutableSortedDictionary<AttributeName, ColAttributes>> Attributes;
        public readonly ImmutableArray<Row> Rows;

        public Sheet(string name, ImmutableSortedDictionary<ColOrder, ImmutableSortedDictionary<AttributeName, ColAttributes>> attributes, ImmutableArray<Row> rows)
        {
            Name = name;
            Attributes = attributes;
            Rows = rows;
        }

        public string? GetColAttribute(ColOrder colOrder, AttributeName name)
        {
            if (Attributes.TryGetValue(colOrder, out var attrs) && attrs.TryGetValue(name, out var attr))
            {
                return attr.Value;
            }
            return null;
        }

        public sealed class SheetAttributesBuilder
        {
            private struct Info
            {
                public int Col;
                public string Name;
                public string Value;
                public override string ToString()
                {
                    return $"({Col})[{Name}]{Value}";
                }
            }
            List<Info> infos = new List<Info>();

            public void Add(int col, string atbName, string atbValue)
            {
                infos.Add(new Info { Col = col, Name = atbName, Value = atbValue });
            }
            public ImmutableSortedDictionary<ColOrder, ImmutableSortedDictionary<AttributeName, ColAttributes>> Build()
            {
                var group = infos.GroupBy(v =>v.Col);
                var a = group.ToImmutableSortedDictionary(kv => new ColOrder(kv.Key), kv =>
                {
                    return kv.ToImmutableSortedDictionary(v => new AttributeName(v.Name), v => new ColAttributes(new AttributeName(v.Name), v.Value),AttributeNameCompare.Instance);
                },ColOrderComparer.Instance);
                return a;
            }
        }
    }
}

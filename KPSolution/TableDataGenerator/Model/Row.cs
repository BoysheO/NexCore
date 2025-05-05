using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableDataGenerator.Model
{
    public sealed class Row
    {
        /// <summary>
        /// [1,+)
        /// </summary>
        public readonly int Order;
        /// <summary>
        /// index:=>col[0,+)
        /// </summary>
        public readonly ImmutableSortedDictionary<ColOrder, string> Values;

        public Row(int order, ImmutableSortedDictionary<ColOrder, string> values)
        {
            Order = order;
            Values = values;
        }

        public override string ToString()
        {
            return $"({Order}){string.Join(',',Values.Values.Take(5))}";
        }

        public static Builder CreatBuilder(int order)
        {
            return new Builder(order);
        }

        public sealed class Builder
        {
            private struct Info
            {
                public int Col;
                public string Value;
            }
            private readonly int Order;
            private List<Info> values = new List<Info>();
            public Builder(int order)
            {
                Order = order;
            }

            public void Add(int col, string value)
            {
                values.Add(new Info()
                {
                    Col = col,
                    Value = value
                });
            }

            public Row Build()
            {
                return new Row(Order, values.ToImmutableSortedDictionary(v => new ColOrder(v.Col), v => v.Value,ColOrderComparer.Instance));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableDataGenerator.Model
{
    public sealed class Book
    {
        public readonly string Name;
        public readonly ImmutableArray<Sheet> Sheets;

        public Book(string name, ImmutableArray<Sheet> sheets)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"“{nameof(name)}”不能为 null 或空白。", nameof(name));
            }

            Name = name;
            Sheets = sheets;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableDataGenerator.Model
{
    public sealed class TableModel
    {
        public readonly struct Field
        {
            public readonly string TypeName;
            public readonly string FieldName;

            public Field(string typeName, string fieldName)
            {
                if (string.IsNullOrEmpty(typeName))
                {
                    throw new ArgumentException($"“{nameof(typeName)}”不能为 null 或空。", nameof(typeName));
                }

                if (string.IsNullOrEmpty(fieldName))
                {
                    throw new ArgumentException($"“{nameof(fieldName)}”不能为 null 或空。", nameof(fieldName));
                }

                TypeName = typeName;
                FieldName = fieldName;
            }
        }
        public readonly string ClassName;
        public readonly ImmutableArray<Field> Fields;
    }
}

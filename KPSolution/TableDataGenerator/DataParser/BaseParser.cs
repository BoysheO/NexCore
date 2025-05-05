using BoysheO.Extensions;
using System.Collections.Immutable;
using System.Numerics;


namespace TableDataGenerator.DataParser
{
    /// <summary>
    /// 处理基础类型和基础数组类型
    /// </summary>
    public sealed class BaseParser
    {
        public sealed class I
        {
            public readonly SortedSet<string> TypeStrings;
            public readonly Type CLRType;
            public readonly Func<string, object> RawValueStringToInstance;
            public readonly string CodeTypeString;

            public I(string[] typeStrings, Type cLRType, Func<string, object> rawValueStringToInstance,
                string codeTypeString)
            {
                CLRType = cLRType;
                RawValueStringToInstance = rawValueStringToInstance;
                CodeTypeString = codeTypeString;
                var lst = new SortedSet<string>();
                foreach (var type in typeStrings)
                {
                    lst.Add(type.ToLowerInvariant());
                }

                var typeName = cLRType.Name.ToLowerInvariant();
                lst.Add(typeName);
                var lowerCodeTypeString = codeTypeString.ToLowerInvariant();
                lst.Add(lowerCodeTypeString);
                TypeStrings = lst;
            }
        }

        List<I> lst = new List<I>()
        {
            // @formatter:off
            new I(new []{"int"},typeof(int),v => v.IsNullOrWhiteSpace()? 0 : int.Parse(v),"int"),
            new I(new []{"long"},typeof(long),v => long.Parse(v),"long"),
            new I(new []{"float"},typeof(float),v=>float.Parse(v),"float"),
            new I(new []{"double"},typeof(double),v=>double.Parse(v),"double"),
            new I(new []{"bool","boolean"},typeof(bool),v=>bool.Parse(v),"bool"),
            new I(new []{"string"},typeof(string),v=>v,"string"),
            new I(new []{"int[]" },typeof(ImmutableArray<int>),v=>v.Split(';','；').Select(v => int.Parse(v)).ToImmutableArray(),"ImmutableArray<int>"),
            new I(new []{"long[]" },typeof(ImmutableArray<long>),v=>v.Split(';','；').Select(v => long.Parse(v)).ToImmutableArray(),"ImmutableArray<long>"),
            new I(new []{"float[]"},typeof(ImmutableArray<float>),v=>v.Split(';','；').Select(v => float.Parse(v)).ToImmutableArray(),"ImmutableArray<float>"),
            new I(new []{"double[]"},typeof(ImmutableArray<double>),v=>v.Split(';','；').Select(v => double.Parse(v)).ToImmutableArray(),"ImmutableArray<double>"),
            new I(new []{"bool[]","boolean[]"},typeof(ImmutableArray<bool>),v=>v.Split(';','；').Select(v => bool.Parse(v)).ToImmutableArray(),"ImmutableArray<bool>"),
            // @formatter:on
        };

        public object Convert(string rawStr, string typeCodeStr)
        {
            try
            {
                var info = lst.First(v => v.TypeStrings.Contains(typeCodeStr.ToLowerInvariant()));
                return info.RawValueStringToInstance(rawStr);
            }
            catch
            {
                Console.WriteLine(new {rawStr, typeCodeStr});
                throw;
            }
        }

        public bool IsTypeStrMatch(string typeStr, out string typeCodeStr)
        {
            var info = lst.FirstOrDefault(v => v.TypeStrings.Contains(typeStr));
            if (info == null)
            {
                typeCodeStr = "";
                return false;
            }

            typeCodeStr = info.CodeTypeString;
            return true;
        }
    }
}
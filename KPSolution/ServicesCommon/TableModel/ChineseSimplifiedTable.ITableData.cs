//auto generate
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TableModels.Abstractions;

namespace TableModels.Models
{
    public partial class ChineseSimplifiedTable:ITableData
    {
        public int Key => Id;

        //inject by tableManager
        [JsonIgnore]        
        private IServiceProvider ServiceProvider { get; set; }

        public int GetInt(string key)
        {
            switch (key)
            {
                case nameof(Id):
                    return Id;
                default:
                    throw new MissingFieldException($"no such field {key} or not int  type");
            }
        }
        public long GetLong(string key)
        {
            switch (key)
            {
                default:
                    throw new MissingFieldException($"no such field {key} or not long  type");
            }
        }
        public double GetDouble(string key)
        {
            switch (key)
            {
                default:
                    throw new MissingFieldException($"no such field {key} or not double  type");
            }
        }
        public bool GetBool(string key)
        {
            switch (key)
            {
                default:
                    throw new MissingFieldException($"no such field {key} or not bool  type");
            }
        }
        public string GetString(string key)
        {
            switch (key)
            {
                case nameof(Text):
                    return Text;
                default:
                    throw new MissingFieldException($"no such field {key} or not string  type");
            }
        }

        public IReadOnlyList<int> GetIntLst(string key)
        {
            switch (key)
            {
                default:
                    throw new MissingFieldException($"no such field {key} or not RepeatedField<int>  type");
            }
        }
        public IReadOnlyList<long> GetLongLst(string key)
        {
            switch (key)
            {
                default:
                    throw new MissingFieldException($"no such field {key} or not RepeatedField<long>  type");
            }
        }
        public IReadOnlyList<double> GetDoubleLst(string key)
        {
            switch (key)
            {
                default:
                    throw new MissingFieldException($"no such field {key} or not RepeatedField<double>  type");
            }
        }
        public IReadOnlyList<bool> GetBoolLst(string key)
        {
            switch (key)
            {
                default:
                    throw new MissingFieldException($"no such field {key} or not RepeatedField<bool>  type");
            }
        }
        public IReadOnlyList<string> GetStringLst(string key)
        {
            switch (key)
            {
                default:
                    throw new MissingFieldException($"no such field {key} or not RepeatedField<string>  type");
            }
        }

        public IReadOnlyList<string> GetKeys()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            return (IReadOnlyList<string>) Descriptor.Fields.InFieldNumberOrder();
        }
    }
}

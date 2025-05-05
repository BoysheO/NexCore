using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BoysheO.Extensions;
using TableModels.Abstractions;
using TableSystem.Abstractions;
using Type = System.Type;
#pragma warning disable CS8603 // Possible null reference return.

namespace HotScripts.Hotfix.GamePlay.ContentSystems.MyJsonSerializerSystem
{
    /// <summary>
    /// 简单地转换
    /// </summary>
    public class TableDataJsonConverter : JsonConverter<ITableData>
    {
        private class Model
        {
            public string TableName { get; set; }
            public int Id { get; set; }
        }

        private readonly ITableDataManager _tableDataManager;
        private readonly Dictionary<string, MethodInfo> _tableName2GenericMethodInfos;

        public TableDataJsonConverter(ITableDataManager tableDataManager)
        {
            _tableDataManager = tableDataManager;

            var method = GetType().GetMethod(nameof(GetTable), BindingFlags.Instance | BindingFlags.NonPublic) ??
                         throw new MissingMethodException();

            MethodInfo MakeGenericMethod(Type type)
            {
                var gmethod = method.MakeGenericMethod(type);
                return gmethod;
            }

            _tableName2GenericMethodInfos = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(v => v.GetTypes())
                .Where(v => v.IsSealedAndImplement(typeof(ITableData)))
                .ToDictionary(v => v.Name, MakeGenericMethod);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsSealedAndImplement(typeof(ITableData));
        }

        public override ITableData Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            var converter = (JsonConverter<Model>)options.GetConverter(typeof(Model));
            var model = converter.Read(ref reader, typeof(Model), options);
            if (model == null) return null;
            var method = _tableName2GenericMethodInfos.GetValueOrDefault(model.TableName);
            if (method == null) return null;
            var o = (ITableData)method.Invoke(this, new object[] { model.Id });
            return o;
        }

        private T? GetTable<T>(int id)
            where T : ITableData
        {
            return _tableDataManager.GetOrNull<T>(id);
        }

        public override void Write(Utf8JsonWriter writer, ITableData value, JsonSerializerOptions options)
        {
            var converter = (JsonConverter<Model>)options.GetConverter(typeof(Model));
            var model = value == null
                ? null
                : new Model()
                {
                    Id = value.Key,
                    TableName = value.GetType().Name
                };
            converter.Write(writer, model, options);
        }
    }
}
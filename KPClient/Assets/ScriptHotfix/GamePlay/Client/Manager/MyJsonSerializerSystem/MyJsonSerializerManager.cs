using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using NexCore.DI;
using TableSystem.Abstractions;

namespace HotScripts.Hotfix.GamePlay.ContentSystems.MyJsonSerializerSystem
{
    [Service(typeof(MyJsonSerializerManager))]
    public class MyJsonSerializerManager
    {
        private readonly JsonSerializerOptions _options;

        public MyJsonSerializerManager(ITableDataManager tableDataManager)
        {
            _options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            _options.Converters.Add(new TableDataJsonConverter(tableDataManager));
        }

        public string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, _options);
        }

        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options) ?? throw new Exception("json fail");
        }
    }
}
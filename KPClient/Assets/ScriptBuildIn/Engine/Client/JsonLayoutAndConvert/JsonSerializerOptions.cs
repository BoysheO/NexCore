using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using BoysheO.Extensions.Unity3D.JsonLayout;
using JsonLayoutAndConvert.Converter;
using UnityEngine;

namespace JsonConfigRepo.Implement
{
    /// <summary>
    /// 默认的Json序列化配置
    /// </summary>
    public static class JsonSerializerOptions
    {
        private static JsonConverter[] CreatStandardConverters()
        {
            return new JsonConverter[]
            {
                new LayoutJsonConverter<Color, LayoutColor>(layout => layout.Primivate,
                    v => new LayoutColor {Primivate = v}),
                new LayoutJsonConverter<Color32, LayoutColor32>(layout => layout.Primivate,
                    v => new LayoutColor32 {Primivate = v}),
                new LayoutJsonConverter<Vector2, LayoutVector2>(layout => layout.Primivate,
                    v => new LayoutVector2 {Primivate = v}),
                new LayoutJsonConverter<Vector3, LayoutVector3>(layout => layout.Primivate,
                    v => new LayoutVector3 {Primivate = v}),
                new LayoutJsonConverter<Vector4, LayoutVector4>(layout => layout.Primivate,
                    v => new LayoutVector4 {Primivate = v}),
                new LayoutJsonConverter<Vector2Int, LayoutVector2Int>(layout => layout.Primivate,
                    v => new LayoutVector2Int {Primivate = v}),
                new LayoutJsonConverter<Vector3Int, LayoutVector3Int>(layout => layout.Primivate,
                    v => new LayoutVector3Int {Primivate = v}),
                new LayoutJsonConverter<Rect, LayoutRect>(layout => layout.Primivate,
                    v => new LayoutRect() {Primivate = v}),
                new LayoutJsonConverter<Quaternion, LayoutQuaternion>(layout => layout.Primivate,
                    v => new LayoutQuaternion() {Primivate = v}),
                new LayoutJsonConverter<Bounds, LayoutBounds>(layout => layout.Primivate,
                    v => new LayoutBounds() {Primivate = v}),
                new VersionConverter(),
                // new ImmutableByteArrayConverter(),
                new JsonStringEnumConverter(),
            };
        }

        /// <summary>
        /// 默认的Json序列化配置库
        /// </summary>
        static JsonSerializerOptions()
        {
            var enumConvert = new JsonStringEnumConverter();
            StandardOptions = new System.Text.Json.JsonSerializerOptions();
            foreach (var converter in CreatStandardConverters()) StandardOptions.Converters.Add(converter);

            UTF8Options = new System.Text.Json.JsonSerializerOptions(StandardOptions)
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            UTF8PrettyOptions = new System.Text.Json.JsonSerializerOptions(UTF8Options)
            {
                WriteIndented = true
            };

            DebugJsonOptions = new System.Text.Json.JsonSerializerOptions(StandardOptions)
            {
                IncludeFields = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };
            if (!DebugJsonOptions.Converters.Contains(enumConvert))
            {
                DebugJsonOptions.Converters.Add(enumConvert);
            }

            DebugJsonPrettyOptions = new System.Text.Json.JsonSerializerOptions(DebugJsonOptions)
            {
                WriteIndented = true
            };
        }

        public static System.Text.Json.JsonSerializerOptions UTF8Options { get; }

        public static System.Text.Json.JsonSerializerOptions UTF8PrettyOptions { get; }

        public static System.Text.Json.JsonSerializerOptions StandardOptions { get; }

        public static System.Text.Json.JsonSerializerOptions DebugJsonOptions { get; }

        public static System.Text.Json.JsonSerializerOptions DebugJsonPrettyOptions { get; }
    }
}
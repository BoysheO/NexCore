#nullable enable
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonLayoutAndConvert.Converter
{
    public sealed class VersionConverter : JsonConverter<Version>
    {
        public override Version? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var ver = reader.GetString();
            return new Version(ver);
        }

        public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
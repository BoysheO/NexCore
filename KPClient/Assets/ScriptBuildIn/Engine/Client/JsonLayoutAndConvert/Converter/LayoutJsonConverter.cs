using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonLayoutAndConvert.Converter
{
    public class LayoutJsonConverter<T, TLayout> : JsonConverter<T>
    {
        private readonly Func<TLayout, T> LayoutToT;
        private readonly Func<T, TLayout> TToLayout;

        public LayoutJsonConverter(Func<TLayout, T> layoutToT, Func<T, TLayout> tToLayout)
        {
            LayoutToT = layoutToT ?? throw new ArgumentNullException(nameof(layoutToT));
            TToLayout = tToLayout ?? throw new ArgumentNullException(nameof(tToLayout));
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var converter = (JsonConverter<TLayout>) options.GetConverter(typeof(TLayout));
            var ins = converter.Read(ref reader, typeof(TLayout), options);
            return LayoutToT(ins!);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var converter = (JsonConverter<TLayout>) options.GetConverter(typeof(TLayout));
            converter.Write(writer, TToLayout(value), options);
        }
    }
}
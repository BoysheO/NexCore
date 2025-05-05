// using System;
// using System.Collections.Immutable;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using BoysheO.Extensions;
//
// namespace JsonLayoutAndConvert.Converter
// {
//     public class ImmutableByteArrayConverter : JsonConverter<ImmutableArray<byte>>
//     {
//         public override ImmutableArray<byte> Read(ref Utf8JsonReader reader, Type typeToConvert,
//             JsonSerializerOptions options)
//         {
//             throw new NotSupportedException();
//         }
//
//         public override void Write(Utf8JsonWriter writer, ImmutableArray<byte> value, JsonSerializerOptions options)
//         {
//             var str = value.ToHexText();
//             writer.WriteStringValue(str);
//         }
//     }
// }
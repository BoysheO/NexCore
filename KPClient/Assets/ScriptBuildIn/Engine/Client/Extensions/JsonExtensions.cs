using System.Text.Json;
using JsonSerializerOptions = JsonConfigRepo.Implement.JsonSerializerOptions;

namespace NexCore.Extensions
{
    public static class JsonExtensions
    {
        public static string ToDebugJson<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj, JsonSerializerOptions.DebugJsonOptions);
        }

        public static string ToJson<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj, JsonSerializerOptions.StandardOptions);
        }
    }
}
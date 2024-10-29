using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace Maliwan.Domain.Core.Extensions;

public static class DistributedCacheExtensions
{
    public static Task SetObjectAsync(this IDistributedCache cache, string key, object value, CancellationToken token = default)
    {
        return SetObjectAsync(cache, key, value, new DistributedCacheEntryOptions(), token);
    }
    public static Task SetObjectAsync(this IDistributedCache cache, string key, object value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, GetJsonSerializerOptions()));
        return cache.SetAsync(key, bytes, options, token);
    }
    public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value)
    {
        var val = cache.Get(key);
        value = default;
        if (val == null) return false;
        value = JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());
        return true;
    }
    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }
}
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using FluentAssertions;
using Flurl.Util;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace Maliwan.Test.Extensions;

public static class HttpClientExtensions
{
    public static HttpClient AddToken(this HttpClient client, string token)
    {
        client = client.AddJsonMediaType();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static HttpClient RemoveToken(this HttpClient client)
    {
        client = client.AddJsonMediaType();
        client.DefaultRequestHeaders.Authorization = null;
        return client;
    }

    public static HttpClient AddJsonMediaType(this HttpClient client)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.AddLanguage();
        return client;
    }

    public static HttpClient AddLanguage(this HttpClient client, LanguageEnum language = LanguageEnum.Portugues)
    {
        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language.GetAttributeOfType<DescriptionAttribute>().Description));
        return client;
    }

    //public async static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(this HttpClient client, string? requestUri,
    //    TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    //{
    //    JsonContent content = JsonContent.Create(value, mediaType: null, options);
    //    var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
    //    {
    //        Content = content
    //    };

    //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    //    return await client.SendAsync(request, cancellationToken);
    //}

    public async static Task<TValue> GetFromJsonAsync<TRequest, TValue>(this HttpClient client, string? requestUri,
        TRequest request, CancellationToken cancellationToken = default)
    where TRequest : class
    {
        var stringParams = JsonConvert.SerializeObject(request, settings: new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        var queryString = new StringBuilder();

        var index = 0;
        foreach (var item in JObject.Parse(stringParams))
        {
            var type = request.GetType().Properties().FirstOrDefault(x => x.Name == item.Key)?.PropertyType;
            var key = item.Key;
            var value = item.Value.Type != JTokenType.Date
                ? (item.Value.Type != JTokenType.Array ? item.Value.ToString() : "")
                : item.Value.ToObject<DateTime>().ToString("O");

            if (item.Value.Type == JTokenType.Array)
            {
                key = $"{key}[]";
                value = String.Join(",", item.Value.ToObject(type));
            }

            queryString.Append($"{(index > 0 ? "&" : "")}{key}={value}");
            index++;
        }
            
            //string.Join("&", .ToKeyValuePairs().Select(q => $"{q.Key}={(q.Value.GetType() == typeof(DateTime) ? ((DateTime) q.Value).ToString("O") : q.Value)}"));

        return await client.GetFromJsonAsync<TValue>($"{requestUri}?{queryString}");
    }
}
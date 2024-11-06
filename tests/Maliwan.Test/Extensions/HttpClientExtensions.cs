using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Flurl.Util;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Extensions;

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

        var queryString = string.Join("&", JsonConvert.DeserializeObject(stringParams).ToKeyValuePairs().Select(q => $"{q.Key}={q.Value}"));

        return await client.GetFromJsonAsync<TValue>($"{requestUri}?{queryString}");
    }
}
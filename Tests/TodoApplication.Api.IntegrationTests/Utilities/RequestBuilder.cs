using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace TodoApplication.Api.IntegrationTests.Utilities;

public class RequestBuilder
{
    private string _route;
    
    private readonly HttpClient _httpClient;
    private readonly QueryParamBuilder _queryParamBuilder;

    private string Url => $"{_route}{_queryParamBuilder.Build()}";

    public RequestBuilder(HttpClient client)
    {
        _httpClient = client;
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
        
        _route = string.Empty;
        _queryParamBuilder = new QueryParamBuilder();
    }
    
    public RequestBuilder SetRoute(string route)
    {
        _route = route;

        return this;
    }

    public RequestBuilder AddQueryParams(string key, string value)
    {
        _queryParamBuilder.Add(key, value);
        
        return this;
    }
    
    public async Task<(HttpStatusCode, TResponse?)> GetAsync<TResponse>()
    {
        var httpResponseMessage = await _httpClient.GetAsync(Url);
        
        return httpResponseMessage.IsSuccessStatusCode
            ? (httpResponseMessage.StatusCode, await JsonUtils.DeserializeAsync<TResponse>(httpResponseMessage.Content.ReadAsStreamAsync()))
            : (httpResponseMessage.StatusCode, default);
    }
    
    public async Task<(HttpStatusCode, TResponse?)> PostAsync<TResponse>(MultipartFormDataContent content)
    {
        var httpResponseMessage = await _httpClient.PostAsync(Url, content);
        
        return httpResponseMessage.IsSuccessStatusCode
            ? (httpResponseMessage.StatusCode, await JsonUtils.DeserializeAsync<TResponse>(httpResponseMessage.Content.ReadAsStreamAsync()))
            : (httpResponseMessage.StatusCode, default);
    }
}
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TodoApplication.Api.IntegrationTests.Utilities;

public static class JsonUtils
{
    private static readonly JsonSerializerOptions? JsonSerializerOptions = SerializerOptions;

    private static JsonSerializerOptions SerializerOptions
    {
        get
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        
            return options;
        }
    }
        
    public static async Task<T?> DeserializeAsync<T>(Task<Stream> stream)
    {
        return await JsonSerializer.DeserializeAsync<T>(await stream, JsonSerializerOptions);
    }
}
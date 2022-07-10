using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoApplication.Api.IntegrationTests.Utilities;

public class QueryParamBuilder
{
    private readonly Dictionary<string, string> _fields;

    public QueryParamBuilder()
    {
        _fields = new Dictionary<string, string>();
    }
    
    public QueryParamBuilder Add(string key, string value)
    {
        _fields.Add(key, value);
        
        return this;
    }
    
    public string Build()
    {
        return _fields.Count == 0
            ? string.Empty
            : $"?{string.Join("&", _fields.Select(pair => $"{HttpUtility.UrlEncode(pair.Key)}={HttpUtility.UrlEncode(pair.Value)}"))}";
    }
}
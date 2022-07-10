using System.Text.Json.Serialization;

namespace TodoApplication.Api.Models.Pagination;

public class PagedResource<TResource>
{
    [JsonPropertyName("_links")]
    public Links Links { get;  }
    public int PageSize { get; }
    public int CurrentPage { get; }
    public int TotalPage { get; }
    public List<TResource> Data { get; }

    public PagedResource(int pageSize, int currentPage, int totalPage, Links links, List<TResource> data)
    {
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPage = totalPage;
        Links = links;
        Data = data;
    }
}
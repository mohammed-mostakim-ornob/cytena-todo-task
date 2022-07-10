using TodoApplication.Api.Models.Pagination;

namespace TodoApplication.Api.Helpers;

public class PaginationHelper
{
    public static PagedResource<TResource> CreatePagedResponse<TResource>(List<TResource> resource, string route, long totalResourceCount, int currentPage, int pageSize)
    {
        var totalPageCount = Convert.ToInt32(Math.Ceiling((double)totalResourceCount / pageSize));
        var nextPageRoute = GenerateNextPageLink(route, currentPage, pageSize, totalPageCount);
        var previousPageRoute = GeneratePreviousPageLink(route, currentPage, pageSize, totalPageCount);
        var selfPageRoute = $"{route}?pageNumber={currentPage}&pageSize={pageSize}";
        var firstPageRoute = $"{route}?pageNumber=1&pageSize={pageSize}";
        var lastPageRoute = $"{route}?pageNumber={totalPageCount}&pageSize={pageSize}";

        return new PagedResource<TResource>(
            pageSize,
            currentPage,
            totalPageCount,
            new Links(firstPageRoute, previousPageRoute, selfPageRoute, nextPageRoute, lastPageRoute),
            resource
        );
    }

    private static string? GenerateNextPageLink(string route, int currentPage, int pageSize, int totalPageCount)
    {
        return currentPage >= 1 && currentPage < totalPageCount
            ? $"{route}?pageNumber={currentPage + 1}&pageSize={pageSize}"
            : null;
    }
    
    private static string? GeneratePreviousPageLink(string route, int currentPage, int pageSize, int totalPageCount)
    {
        return currentPage - 1 >= 1 && currentPage <= totalPageCount
            ? $"{route}?pageNumber={currentPage - 1}&pageSize={pageSize}"
            : null;
    }
}
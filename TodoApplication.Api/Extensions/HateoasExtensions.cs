using RiskFirst.Hateoas;
using RiskFirst.Hateoas.Models;

namespace TodoApplication.Api.Extensions;

public static class HateoasExtensions
{
    public static async Task AddLinksAsync<TItem>(this List<TItem> items, ILinksService linksService) where TItem : ILinkContainer
    {
        foreach (var it in items)
        {
            await linksService.AddLinksAsync(it);
        }
    }
}
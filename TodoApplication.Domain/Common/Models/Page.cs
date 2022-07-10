namespace TodoApplication.Domain.Common.Models;

public readonly struct Page<TEntity>
{
    public List<TEntity> Items { get; }

    public long TotalItemCount { get; }

    public int CurrentPageIndex { get; }
    
    public int PageSize { get; }

    public int TotalPages { get; }

    public bool HasNextPage => (CurrentPageIndex < TotalPages);

    public bool HasPreviousPage => (CurrentPageIndex > 1);

    public int? NextPageIndex => HasNextPage ? (int?)(CurrentPageIndex + 1) : null;

    public int? PreviousPageIndex => HasPreviousPage ? (int?)(CurrentPageIndex - 1) : null;

    public Page(List<TEntity> items, long count, int pageIndex, int pageSize)
    {
        Items = items;
        TotalItemCount = count;
        CurrentPageIndex = pageIndex;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}
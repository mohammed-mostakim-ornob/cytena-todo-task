namespace TodoApplication.Api.Models.Pagination;

public class Links
{
    public string First { get; }
    public string? Prev { get; }
    public string Self { get; }
    public string? Next { get; }
    public string Last { get; }

    public Links(string first, string? prev, string self, string? next, string last)
    {
        First = first;
        Prev = prev;
        Self = self;
        Next = next;
        Last = last;
    }
}
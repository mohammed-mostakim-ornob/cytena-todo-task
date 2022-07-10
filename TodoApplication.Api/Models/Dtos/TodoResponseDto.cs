using RiskFirst.Hateoas.Models;
using TodoApplication.Domain.Todo.Models;

namespace TodoApplication.Api.Models.Dtos;

public class TodoResponseDto : ILinkContainer
{
    public long Id { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public Status Status { get; set; }
    public ImageDto? Image { get; set; }

    public Dictionary<string, Link> Links { get; set; } = new();

    public void AddLink(string id, Link link)
    {
        Links.Add(id, link);
    }
}
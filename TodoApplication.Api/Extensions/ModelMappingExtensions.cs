using TodoApplication.Api.Models.Dtos;
using TodoApplication.Domain.Todo.Models;

namespace TodoApplication.Api.Extensions;

public static class ModelMappingExtensions
{
    public static TodoResponseDto MapToTodoResponseDto(this Todo todo, string? imageHref)
    {
        return new TodoResponseDto()
        {
            Id = todo.Id,
            Description = todo.Description,
            DueDate = todo.DueDate.ToLocalTime(),
            Status = todo.Status,
            Image = todo.TodoImage == null ? null : new ImageDto()
            {
                Name = todo.TodoImage.Name,
                Src = imageHref
            }
        };
    }
}
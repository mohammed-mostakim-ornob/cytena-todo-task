using TodoApplication.Domain.Common.Models;
using TodoApplication.Domain.Todo.Models;

namespace TodoApplication.Domain.Todo.Services.Interfaces;

public interface ITodoService
{
    public Task<Models.Todo> GetTodoAsync(long id);

    public Task<Page<Models.Todo>> GetAllTodosPagedAsync(int pageNumber, int pageSize);

    public Task<Models.Todo> AddTodoAsync(string description, DateTime dueDate, Status status, string? fileName, Stream? imageFileStream);

    public Task<TodoImage> GetTodoImageByTodoIdAsync(long todoId);
}
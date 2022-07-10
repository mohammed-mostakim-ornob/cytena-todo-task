using TodoApplication.Domain.Common;
using TodoApplication.Domain.Common.Models;

namespace TodoApplication.Domain.Todo.Models;

public class Todo : EntityBase<long>
{
    public string Description { get; }
    public DateTime DueDate { get; }
    public Status Status { get; }
    public TodoImage? TodoImage { get; }

    public Todo()
    { }

    public Todo(string description, DateTime dueDate, Status status, TodoImage? todoImage)
    {
        Description = description;
        DueDate = dueDate;
        Status = status;
        TodoImage = todoImage;
    }
}
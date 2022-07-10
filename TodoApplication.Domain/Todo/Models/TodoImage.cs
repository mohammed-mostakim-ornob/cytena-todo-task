using TodoApplication.Domain.Common;
using TodoApplication.Domain.Common.Models;

namespace TodoApplication.Domain.Todo.Models;

public class TodoImage : EntityBase<long>
{
    public string Name { get; }
    public string Path { get; }
    public Todo Todo { get; }

    public TodoImage()
    { }
    
    public TodoImage(string name, string path)
    {
        Name = name;
        Path = path;
    }
}
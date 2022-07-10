using TodoApplication.Domain.Todo.Repositories;
using TodoApplication.Infrastructure.DbContexts.Todo;

namespace TodoApplication.Infrastructure.Repositories.Todo;

public class TodoRepository : TodoGenericRepository<Domain.Todo.Models.Todo, long>, ITodoRepository
{
    public TodoRepository(TodoDbContext todoDbContext)
        : base(todoDbContext) { }
}
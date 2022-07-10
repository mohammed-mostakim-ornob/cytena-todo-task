using Microsoft.EntityFrameworkCore;
using TodoApplication.Infrastructure.DbContexts.Todo;

namespace TodoApplication.Infrastructure.IntegrationTests.Repositories;

public class RepositoryIntegrationTestBase
{
    protected readonly TodoDbContext TodoDbContext;
    
    protected RepositoryIntegrationTestBase()
    {
        var dbContextOptions = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: "TodoDbRepositoryTesting")
            .Options;

        TodoDbContext = new TodoDbContext(dbContextOptions);
    }
}
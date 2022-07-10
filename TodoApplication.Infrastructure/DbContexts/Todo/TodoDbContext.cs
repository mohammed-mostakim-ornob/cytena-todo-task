using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TodoApplication.Domain.Todo.Models;
using TodoApplication.Infrastructure.Mappings;

namespace TodoApplication.Infrastructure.DbContexts.Todo;

public class TodoDbContext : DbContext
{
    public DbSet<Domain.Todo.Models.Todo> Todos { get; set; }
    public DbSet<TodoImage> TodoImages { get; set; }
    
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureMappings(modelBuilder);
    }
    
    private void ConfigureMappings(ModelBuilder modelBuilder)
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => typeof(IEntityMap).IsAssignableFrom(x) && x.IsClass)
            .ToList()
            .ForEach(x =>
            {
                Activator.CreateInstance(
                    x,
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new object[] { modelBuilder },
                    null);
            });
    }
}
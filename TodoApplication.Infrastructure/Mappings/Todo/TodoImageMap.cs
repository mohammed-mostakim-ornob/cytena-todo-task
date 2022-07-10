using Microsoft.EntityFrameworkCore;
using TodoApplication.Domain.Todo.Models;

namespace TodoApplication.Infrastructure.Mappings.Todo;

public class TodoImageMap : IEntityMap
{
    public TodoImageMap(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoImage>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<Domain.Todo.Models.Todo>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<TodoImage>()
            .Property(x => x.Name)
            .IsRequired();
        
        modelBuilder.Entity<TodoImage>()
            .Property(x => x.Path)
            .IsRequired();
    }
}
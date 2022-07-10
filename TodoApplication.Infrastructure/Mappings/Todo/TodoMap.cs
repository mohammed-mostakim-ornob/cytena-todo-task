using Microsoft.EntityFrameworkCore;
using TodoApplication.Domain.Todo.Models;

namespace TodoApplication.Infrastructure.Mappings.Todo;

public class TodoMap : IEntityMap
{
    public TodoMap(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Todo.Models.Todo>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<Domain.Todo.Models.Todo>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Domain.Todo.Models.Todo>()
            .Property(x => x.Description)
            .IsRequired();
        
        modelBuilder.Entity<Domain.Todo.Models.Todo>()
            .Property(x => x.DueDate)
            .IsRequired();
        
        modelBuilder.Entity<Domain.Todo.Models.Todo>()
            .Property(x => x.Status)
            .HasConversion(
                v => v.ToString(),
                v => (Status)Enum.Parse(typeof(Status), v))
            .IsRequired();
        
        modelBuilder.Entity<Domain.Todo.Models.Todo>()
            .HasOne(x => x.TodoImage)
            .WithOne(x => x.Todo)
            .HasForeignKey<TodoImage>(x => x.Id);
    }
}
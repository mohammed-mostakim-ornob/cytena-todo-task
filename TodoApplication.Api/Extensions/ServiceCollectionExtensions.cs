using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RiskFirst.Hateoas;
using TodoApplication.Api.Configurations;
using TodoApplication.Api.Models.Dtos;
using TodoApplication.Domain.File.Services;
using TodoApplication.Domain.Todo.Repositories;
using TodoApplication.Domain.Todo.Services.Implementations;
using TodoApplication.Domain.Todo.Services.Interfaces;
using TodoApplication.Infrastructure.DbContexts.Todo;
using TodoApplication.Infrastructure.Repositories.Todo;
using TodoApplication.Infrastructure.Services.File;

namespace TodoApplication.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string GetSection(string name) => configuration.GetSection($"Databases:Todo:{name}").Value;

        var databaseInfo = new DatabaseInfo(
            host: GetSection("Host"),
            port: GetSection("Port"),
            username: GetSection("UserName"),
            password: GetSection("Password"),
            database: GetSection("DatabaseName"),
            disableSsl: true
        );
        
        var migrationsAssembly = typeof(TodoDbContext).GetTypeInfo().Assembly.GetName().Name;

        services.AddDbContext<TodoDbContext>(
            builder => builder.UseNpgsql(databaseInfo.ConnectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly(migrationsAssembly);
                optionsBuilder.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
            }));
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<ITodoService, TodoService>();
        services.AddTransient<IFileService, FileService>();
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ITodoRepository, TodoRepository>();
    }

    public static void AddHateoasPolicies(this IServiceCollection services)
    {
        services.AddLinks(config => 
        {
            config.AddPolicy<TodoResponseDto>(policy => {
                policy.RequireRoutedLink("self", "GetTodoItem", x => new { id = x.Id });
            });
        });
    }
}
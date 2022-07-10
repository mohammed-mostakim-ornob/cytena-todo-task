using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using TodoApplication.Infrastructure.DbContexts.Todo;

namespace TodoApplication.Api.IntegrationTests.Infrastructure;

public class ApiIntegrationTestFixture : IDisposable
{
    public readonly TodoDbContext? DbContext;
    private readonly TestingWebApplicationFactory _factory;

    public ApiIntegrationTestFixture()
    {
        _factory = new TestingWebApplicationFactory();

        DbContext = _factory.Services.CreateScope().ServiceProvider.GetService<TodoDbContext>();
    }
    public void Dispose()
    {
        DbContext?.Dispose();
        _factory.Dispose();
    }

    public HttpClient HttpClient => _factory.CreateClient();
}
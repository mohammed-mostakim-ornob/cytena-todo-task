using System.Net.Http;
using TodoApplication.Api.IntegrationTests.Utilities;
using TodoApplication.Infrastructure.DbContexts.Todo;
using Xunit;

namespace TodoApplication.Api.IntegrationTests.Infrastructure;

public class ApiIntegrationTestBase : IClassFixture<ApiIntegrationTestFixture>
{
    private TodoDbContext? TodoDbContext { get; }

    private ApiIntegrationTestFixture _fixture;
    
    protected ApiIntegrationTestBase(ApiIntegrationTestFixture fixture)
    {
        _fixture = fixture;
        TodoDbContext = fixture.DbContext;

        if (TodoDbContext == null) return;
        
        TodoDbContext.Database.EnsureDeleted();
        TodoDbContext.Database.EnsureCreated();
    }

    public HttpClient HttpClient => _fixture.HttpClient;

    public RequestBuilder NewRequest => new(_fixture.HttpClient);
}
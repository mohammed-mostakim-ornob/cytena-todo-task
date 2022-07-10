using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApplication.Infrastructure.DbContexts.Todo;

namespace TodoApplication.Api.IntegrationTests.Infrastructure;

public class TestingWebApplicationFactory : WebApplicationFactory<Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<TodoDbContext>));
            
            if (descriptor != null)
                services.Remove(descriptor);
            
            services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseInMemoryDatabase("TodoDbApiTesting");
            });
        });
    }
}
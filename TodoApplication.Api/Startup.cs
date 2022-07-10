using System.Text.Json.Serialization;
using TodoApplication.Api.Extensions;
using TodoApplication.Api.Filters;
using TodoApplication.Infrastructure.DbContexts.Todo;
using FileOptions = TodoApplication.Domain.File.FileOptions;

namespace TodoApplication.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllers(options => options.Filters.Add<CustomExceptionFilter>())
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        
        services.AddDatabase(_configuration);
        
        services.AddServices();
        
        services.AddRepositories();
        
        services.AddHateoasPolicies();
        
        services.Configure<FileOptions>(_configuration.GetSection(FileOptions.File));
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => options.DescribeAllParametersInCamelCase());

    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TodoDbContext dbContext)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.UseHttpsRedirection();
        app.UseStaticFiles();
    }
}
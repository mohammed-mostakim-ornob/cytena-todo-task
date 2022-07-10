using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using TodoApplication.Api.IntegrationTests.Infrastructure;
using TodoApplication.Api.Models.Dtos;
using TodoApplication.Api.Models.Pagination;
using TodoApplication.Domain.Todo.Models;
using TodoApplication.Tests.Common.Helpers;
using Xunit;

namespace TodoApplication.Api.IntegrationTests;

public class TodosControllerTests : ApiIntegrationTestBase
{
    private const string TodoControllerEndpoint = "/api/todos";
    
    public TodosControllerTests(ApiIntegrationTestFixture fixture) : base(fixture)
    {
        
    }
    
    [Fact]
    public async Task GetTodoItem_Should_Return_200()
    {
        //setup
        var description = "mock description";
        var dueDate = new DateTime(2022, 10, 22);
        var status = Status.Pending;
        
        var (_, expectedTodo) = await CreateTodoAsync(description, dueDate, status);
        
        // act
        var (resultStatusCode, resultContent) = await NewRequest.SetRoute($"{TodoControllerEndpoint}/{expectedTodo?.Id}")
            .GetAsync<TodoResponseDto>();

        // assert
        resultStatusCode.Should().Be(HttpStatusCode.OK);
        resultContent?.Id.Should().Be(expectedTodo?.Id);
    }

    [Fact]
    public async Task GetTodoItem_Should_Return_404()
    {
        // act
        var (resultStatusCode, _) = await NewRequest.SetRoute($"{TodoControllerEndpoint}/{long.MaxValue}")
            .GetAsync<TodoResponseDto>();
        
        // assert
        resultStatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllTodoItems_Should_Return_200()
    {
        //setup
        var itemCount = 5;
        var description = "mock description";
        var dueDate = new DateTime(2022, 10, 22);
        var status = Status.Pending;

        for (int i = 0; i < itemCount; i++)
        {
            var (_, expectedTodo) = await CreateTodoAsync(description, dueDate, status);
        }
        
        // act
        var (resultStatusCode, resultContent) = await NewRequest.SetRoute(TodoControllerEndpoint)
            .GetAsync<PagedResource<TodoResponseDto>>();
        
        // assert
        resultStatusCode.Should().Be(HttpStatusCode.OK);
        resultContent?.Data.Count.Should().Be(5);
    }

    [Theory]
    [InlineData(0,0)]
    [InlineData(1,0)]
    [InlineData(0,1)]
    public async Task GetAllTodoItems_Should_Return_400(int pageSize, int pageNumber)
    {
        // act
        var (resultStatusCode, _) = await NewRequest.SetRoute(TodoControllerEndpoint)
            .AddQueryParams("pageSize", pageNumber.ToString())
            .AddQueryParams("pageNumber", pageSize.ToString())
            .GetAsync<PagedResource<TodoResponseDto>>();
        
        // assert
        resultStatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTodo_Should_Return_201()
    {
        //setup
        var description = "mock description";
        var dueDate = new DateTime(2022, 10, 22);
        var status = Status.Pending;
        
        // act
        var (resultStatusCode, resultContent) = await CreateTodoAsync(description, dueDate, status);

        // assert
        resultStatusCode.Should().Be(HttpStatusCode.Created);
        resultContent?.Id.Should().NotBe(0);
        resultContent?.Description.Should().Be(description);
        resultContent?.DueDate.Should().Be(dueDate);
        resultContent?.Status.Should().Be(status);
    }

    [Theory]
    [MemberData(nameof(InvalidTodoData))]
    public async Task AddTodo_Should_Return_400(bool isAddDescription, bool isAddStatus, bool isAddDueDate, bool isAddPastDate)
    {
        // setup
        var description = "mock description";
        var pastDate = new DateTime(2021, 10, 22);
        var dueDate = new DateTime(2022, 10, 22);
        var status = Status.Pending;
        
        using var formData = new MultipartFormDataContent();
        
        if (isAddDescription) formData.Add(new StringContent(description), "Description");
        if (isAddStatus) formData.Add(new StringContent(Enum.GetName(typeof(Status), status)), "Status");

        if (isAddDueDate)
        {
            if (isAddPastDate) formData.Add(new StringContent(pastDate.ToString("MM-dd-yyyy HH:mm:ss")), "DueDate");
            else formData.Add(new StringContent(dueDate.ToString("MM-dd-yyyy HH:mm:ss")), "DueDate");
        }

        // act
        var (resultStatusCode, _) = await NewRequest
            .SetRoute(TodoControllerEndpoint)
            .PostAsync<TodoResponseDto>(formData);
        
        // assert
        resultStatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTodo_InvalidExtension_Should_Return_400()
    {
        // setup
        var description = "mock description";
        var dueDate = new DateTime(2022, 10, 22);
        var status = Status.Pending;
        
        using var formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent(description), "Description");
        formData.Add(new StringContent(Enum.GetName(typeof(Status), status)), "Status");
        formData.Add(new StringContent(dueDate.ToString("MM-dd-yyyy HH:mm:ss")), "DueDate");
        formData.Add(new StreamContent(MockHelper.GetPngImageStream()), "ImageFile", MockHelper.PngImageName);

        // act
        var (resultStatusCode, _) = await NewRequest
            .SetRoute(TodoControllerEndpoint)
            .PostAsync<TodoResponseDto>(formData);
        
        // assert
        resultStatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task AddTodo_TooLargeFile_Should_Return_400()
    {
        // setup
        var description = "mock description";
        var dueDate = new DateTime(2022, 10, 22);
        var status = Status.Pending;
        
        using var formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent(description), "Description");
        formData.Add(new StringContent(Enum.GetName(typeof(Status), status)), "Status");
        formData.Add(new StringContent(dueDate.ToString("MM-dd-yyyy HH:mm:ss")), "DueDate");
        formData.Add(new StreamContent(MockHelper.GetTooLargeImageStream()), "ImageFile", MockHelper.TooLargeImageName);

        // act
        var (resultStatusCode, _) = await NewRequest
            .SetRoute(TodoControllerEndpoint)
            .PostAsync<TodoResponseDto>(formData);
        
        // assert
        resultStatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTodoImage_Should_Return_ResponseCode200()
    {
        //setup
        var description = "mock description";
        var dueDate = new DateTime(2022, 10, 22);
        var status = Status.Pending;
        
        var (_, expectedTodo) = await CreateTodoAsync(description, dueDate, status);
        
        // act
        var httpResponseMessage = await HttpClient.GetAsync($"{TodoControllerEndpoint}/{expectedTodo?.Id}/image");

        // assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetTodoImage_Should_Return_ResponseCode404()
    {
        // act
        var httpResponseMessage = await HttpClient.GetAsync($"{TodoControllerEndpoint}/{long.MaxValue}/image");

        // assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<(HttpStatusCode StatusCode, TodoResponseDto?)> CreateTodoAsync(string description, DateTime dueDate, Status status, bool isEmptyImage = false)
    {
        using var formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent(description), "Description");
        formData.Add(new StringContent(dueDate.ToString("MM-dd-yyyy HH:mm:ss")), "DueDate");
        formData.Add(new StringContent(Enum.GetName(typeof(Status), status)), "Status");
            
        if (!isEmptyImage)
            formData.Add(new StreamContent(MockHelper.GetMockImageStream()), "ImageFile", MockHelper.MockImageName);

        return await NewRequest
            .SetRoute(TodoControllerEndpoint)
            .PostAsync<TodoResponseDto>(formData);
    }
    
    public static IEnumerable<object[]> InvalidTodoData()
    {
        yield return new object[] { false, false, false, false };
        yield return new object[] { false, true, true, false };
        yield return new object[] { true, true, false, false };
        yield return new object[] { true, true, true, true };
    }
}
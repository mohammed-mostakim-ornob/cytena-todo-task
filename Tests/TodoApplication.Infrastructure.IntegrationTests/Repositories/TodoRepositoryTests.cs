using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoApplication.Domain.Todo.Models;
using TodoApplication.Infrastructure.Repositories.Todo;
using Xunit;

namespace TodoApplication.Infrastructure.IntegrationTests.Repositories;

public class TodoRepositoryTests : RepositoryIntegrationTestBase
{
    private readonly TodoRepository _sut;
    
    public TodoRepositoryTests()
    {
        _sut = new TodoRepository(TodoDbContext);

        TodoDbContext.Database.EnsureDeleted();
        TodoDbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAsyncIncluding_Should_Return_Todo()
    {
        // setup
        var newTodo = new Todo("description", DateTime.Now, Status.Pending, new TodoImage("fileName", "filePath"));
        await _sut.AddAsync(newTodo);
        
        // act
        var result = await _sut.GetAsyncIncluding(newTodo.Id, x => x.TodoImage);

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(newTodo.Id);
        result.Description.Should().Be(newTodo.Description);
        result.DueDate.Should().Be(newTodo.DueDate);
        result.Status.Should().Be(newTodo.Status);

        result.TodoImage.Should().NotBeNull();
        result.TodoImage.Name.Should().Be(newTodo.TodoImage.Name);
        result.TodoImage.Path.Should().Be(newTodo.TodoImage.Path);
    }

    [Fact]
    public async Task GetAsyncIncluding_Should_Return_Null()
    {
        // setup
        var id = long.MaxValue;
        
        // act
        var result = await _sut.GetAsyncIncluding(id, x => x.TodoImage);
        
        // assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllPagedAsyncIncluding_Should_Return_PagedTodos()
    {
        // setup
        var pageSize = 2;
        var pageNumber = 2;
        var newTodos = new[]
        {
            new Todo("description", DateTime.Now, Status.Pending, new TodoImage("fileName", "filePath")),
            new Todo("description", DateTime.Now, Status.Pending, new TodoImage("fileName", "filePath")),
            new Todo("description", DateTime.Now, Status.Pending, new TodoImage("fileName", "filePath")),
            new Todo("description", DateTime.Now, Status.Pending, new TodoImage("fileName", "filePath")),
            new Todo("description", DateTime.Now, Status.Pending, new TodoImage("fileName", "filePath"))
        };
        
        foreach (var item in newTodos)
        {
            await _sut.AddAsync(item);
        }
        
        // action
        var result = await _sut.GetAllPagedAsyncIncluding(pageNumber, pageSize, x => x.TodoImage);
        
        // assert
        result.CurrentPageIndex.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalItemCount.Should().Be(newTodos.Length);
        result.Items.Count.Should().Be(pageNumber);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeTrue();
        result.NextPageIndex.Should().Be(pageNumber + 1);
        result.PreviousPageIndex.Should().Be(pageNumber - 1);
        result.TotalPages.Should().Be((newTodos.Length + pageSize - 1) / pageSize);
    }

    [Fact]
    public async Task GetAllPagedAsyncIncluding_Should_Return_NoItem()
    {
        // setup
        var pageSize = 1;
        var pageNumber = 1;
        
        // action
        var result = await _sut.GetAllPagedAsyncIncluding(pageNumber, pageSize, x => x.TodoImage);

        result.Items.Count.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.TotalItemCount.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
        result.NextPageIndex.Should().BeNull();
        result.PreviousPageIndex.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_Should_AddTodo()
    {
        // setup
        var newTodo = new Todo("description", DateTime.Now, Status.Pending, new TodoImage("fileName", "filePath"));
        
        // act
        await _sut.AddAsync(newTodo);
        
        // assert
        newTodo.Id.Should().BeGreaterThan(0);
    }

    [Theory]
    [MemberData(nameof(InvalidTodoData))]
    public async Task AddAsync_Should_Throw_DbUpdateException(string description, DateTime dueDate, Status status, string fileName, string filePath)
    {
        // setup
        var newTodo = new Todo(description, dueDate, status, new TodoImage(fileName, filePath));
        
        // act
        Func<Task> act = () => _sut.AddAsync(newTodo);

        // assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }
    
    public static IEnumerable<object[]> InvalidTodoData()
    {
        yield return new object[] { null, DateTime.Now, Status.Pending, "fileName", "filePath" };
        yield return new object[] { "description", DateTime.Now, Status.Pending, null, "filePath" };
        yield return new object[] { "description", DateTime.Now, Status.Pending, "fileName", null };
    }
}
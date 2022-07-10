using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using TodoApplication.Common.Exceptions;
using TodoApplication.Domain.Common.Models;
using TodoApplication.Domain.File.Services;
using TodoApplication.Domain.Todo.Models;
using TodoApplication.Domain.Todo.Repositories;
using TodoApplication.Domain.Todo.Services.Implementations;
using TodoApplication.Tests.Common.Helpers;
using Xunit;
using FileOptions = TodoApplication.Domain.File.FileOptions;

namespace TodoApplication.Domain.UnitTests;

public class TodoServiceTests
{
    private readonly Fixture _fixture;
    
    private readonly IOptions<FileOptions> _fileOptions;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<ITodoRepository> _mockTodoRepository;

    private readonly TodoService _sut;
    
    public TodoServiceTests()
    {
        _fixture = new Fixture();
        
        _fileOptions = Options.Create(new FileOptions()
        {
            TodoImageDirectory = "TestDirectory"
        });
        _mockFileService = new Mock<IFileService>();
        _mockTodoRepository = new Mock<ITodoRepository>();

        _sut = new TodoService(_fileOptions, _mockFileService.Object, _mockTodoRepository.Object);
    }

    [Fact]
    public async Task GetTodoAsync_Should_Return_Todo()
    {
        // setup
        var expected = _fixture.Create<Todo.Models.Todo>();

        _mockTodoRepository.Setup(tr => tr.GetAsyncIncluding(It.IsAny<long>(), It.IsAny<Expression<Func<Todo.Models.Todo, object>>[]>()))
            .ReturnsAsync(expected)
            .Verifiable();

        // act
        var result = await _sut.GetTodoAsync(expected.Id);

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expected.Id);
        
        _mockTodoRepository.Verify(tr => tr.GetAsyncIncluding(expected.Id, x => x.TodoImage));
    }

    [Fact]
    public async Task GetTodoAsync_Should_Throw_ResourceNotFoundException()
    {
        // setup
        var id = _fixture.Create<long>();

        _mockTodoRepository.Setup(tr => tr.GetAsyncIncluding(It.IsAny<long>(), It.IsAny<Expression<Func<Todo.Models.Todo, object>>[]>()))
            .ReturnsAsync((Todo.Models.Todo)null)
            .Verifiable();
        
        // act
        Func<Task> act = () => _sut.GetTodoAsync(id);

        // assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
        
        _mockTodoRepository.Verify(tr => tr.GetAsyncIncluding(id, x => x.TodoImage));
    }

    [Fact]
    public async Task GetAllTodosPagedAsync_Should_Return_PagedTodos()
    {
        // setup
        var pageNumber = 1;
        var pageSize = 5;
        var items = new List<Todo.Models.Todo>()
        {
            _fixture.Create<Todo.Models.Todo>(),
            _fixture.Create<Todo.Models.Todo>()
        };
        var expected = new Page<Todo.Models.Todo>(items, items.Count, pageNumber, pageSize);

        _mockTodoRepository.Setup(tr => tr.GetAllPagedAsyncIncluding(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Todo.Models.Todo, object>>[]>()))
            .ReturnsAsync(expected)
            .Verifiable();
        
        // act
        var result = await _sut.GetAllTodosPagedAsync(pageNumber, pageSize);
        
        // assert
        result.Items.Count.Should().Be(items.Count);
        result.PageSize.Should().Be(pageSize);
        result.CurrentPageIndex.Should().Be(pageNumber);
        
        _mockTodoRepository.Verify(tr => tr.GetAllPagedAsyncIncluding(pageNumber, pageSize, x => x.TodoImage));
    }

    [Fact]
    public async Task AddTodoAsync_Should_Return_AddedTodo()
    {
        // setup
        var description = "mock description";
        var dueDate = DateTime.Now;
        var status = Status.Pending;
        var mockImage = await MockHelper.GetMockImageAsync();
        var fileName = mockImage.Object.FileName;

        var filePath = Path.Combine(_fileOptions.Value.TodoImageDirectory, fileName);

        var expectedTodo = new Todo.Models.Todo(description, dueDate, status, new TodoImage(fileName, filePath));

        _mockTodoRepository.Setup(tr => tr.AddAsync(It.IsAny<Todo.Models.Todo>()))
            .ReturnsAsync(expectedTodo);
        
        _mockFileService.Setup(fs => fs.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(filePath);
        
        // act
        var result = await _sut.AddTodoAsync(description, dueDate, status, fileName, mockImage.Object.OpenReadStream());
        
        // assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AddTodoAsync_Should_Throw_ArgumentNullException()
    {
        // setup
        var description = "mock description";
        var dueDate = DateTime.Now;
        var status = Status.Pending;
        var mockImage = await MockHelper.GetMockImageAsync();
        string? fileName = null;
        
        // act
        Func<Task> act = () => _sut.AddTodoAsync(description, dueDate, status, fileName, mockImage.Object.OpenReadStream());

        // assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetTodoImageByTodoIdAsync_Should_Return_TodoImage()
    {
        // setup
        var totoId = _fixture.Create<long>();
        var expectedTodoImage = _fixture.Create<TodoImage>();
        
        var todo = new Todo.Models.Todo(
            _fixture.Create<string>(),
            _fixture.Create<DateTime>(),
            _fixture.Create<Status>(),
            expectedTodoImage
        );

        _mockTodoRepository.Setup(tr =>
                tr.GetAsyncIncluding(It.IsAny<long>(), It.IsAny<Expression<Func<Todo.Models.Todo, object>>[]>()))
            .ReturnsAsync(todo)
            .Verifiable();
        
        // act
        var result = await _sut.GetTodoImageByTodoIdAsync(totoId);
        
        // assert
        result.Should().NotBeNull();
        
        _mockTodoRepository.Verify(tr => tr.GetAsyncIncluding(totoId, x => x.TodoImage));
    }

    [Fact]
    public async Task GetTodoImageByTodoIdAsync_Should_Throw_ResourceNotFoundException()
    {
        // setup
        var id = _fixture.Create<long>();

        _mockTodoRepository.Setup(tr => tr.GetAsyncIncluding(It.IsAny<long>(), It.IsAny<Expression<Func<Todo.Models.Todo, object>>[]>()))
            .ReturnsAsync((Todo.Models.Todo)null)
            .Verifiable();
        
        // act
        Func<Task> act = () => _sut.GetTodoImageByTodoIdAsync(id);

        // assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
        
        _mockTodoRepository.Verify(tr => tr.GetAsyncIncluding(id, x => x.TodoImage));
        
        // setup
        var expectedTodo = new Todo.Models.Todo(
            _fixture.Create<string>(),
            _fixture.Create<DateTime>(),
            _fixture.Create<Status>(),
            null
        );
        
        _mockTodoRepository.Setup(tr => tr.GetAsyncIncluding(It.IsAny<long>(), It.IsAny<Expression<Func<Todo.Models.Todo, object>>[]>()))
            .ReturnsAsync(expectedTodo)
            .Verifiable();
        
        // act
        act = () => _sut.GetTodoImageByTodoIdAsync(id);

        // assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
        
        _mockTodoRepository.Verify(tr => tr.GetAsyncIncluding(id, x => x.TodoImage));
    }
}
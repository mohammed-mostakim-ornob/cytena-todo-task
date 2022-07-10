using Microsoft.Extensions.Options;
using TodoApplication.Common.Constants;
using TodoApplication.Common.Exceptions;
using TodoApplication.Domain.Common.Models;
using TodoApplication.Domain.File.Services;
using TodoApplication.Domain.Todo.Models;
using TodoApplication.Domain.Todo.Repositories;
using TodoApplication.Domain.Todo.Services.Interfaces;
using FileOptions = TodoApplication.Domain.File.FileOptions;

namespace TodoApplication.Domain.Todo.Services.Implementations;

public class TodoService : ITodoService
{
    private readonly FileOptions _fileOptions;
    private readonly IFileService _fileService;
    private readonly ITodoRepository _todoRepository;

    private string UserHomeDirectory => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public TodoService(IOptions<FileOptions> options, IFileService fileService, ITodoRepository todoRepository)
    {
        _fileOptions = options.Value;
        _fileService = fileService;
        _todoRepository = todoRepository;
    }

    public async Task<Models.Todo> GetTodoAsync(long id)
    {
        var todo = await _todoRepository.GetAsyncIncluding(id, x => x.TodoImage);

        if (todo == null)
            throw new ResourceNotFoundException(ExceptionMessages.ResourceNotFound);

        return todo;
    }

    public async Task<Page<Models.Todo>> GetAllTodosPagedAsync(int pageNumber, int pageSize)
    {
        return await _todoRepository.GetAllPagedAsyncIncluding(pageNumber, pageSize, x => x.TodoImage);
    }

    public async Task<Models.Todo> AddTodoAsync(string description, DateTime dueDate, Status status, string? fileName, Stream? imageFileStream)
    {
        TodoImage? todoImage = null;
        
        if (imageFileStream != null)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(ExceptionMessages.FileNameNull);

            var imageFilePath = await StoreImageAsync(imageFileStream);

            todoImage = new TodoImage(fileName, imageFilePath);
        }

        var newTodo = new Models.Todo(description, dueDate.ToUniversalTime(), status, todoImage);

        await _todoRepository.AddAsync(newTodo);

        return newTodo;
    }

    public async Task<TodoImage> GetTodoImageByTodoIdAsync(long todoId)
    {
        var todo = await _todoRepository.GetAsyncIncluding(todoId, x => x.TodoImage);

        if (todo?.TodoImage == null)
            throw new ResourceNotFoundException(ExceptionMessages.ResourceNotFound);
        
        return todo.TodoImage;
    }

    private async Task<string> StoreImageAsync(Stream imageFileStream)
    {
        var directory = UserHomeDirectory + _fileOptions.TodoImageDirectory;

        return await _fileService.SaveAsync(imageFileStream, directory);
    }
}
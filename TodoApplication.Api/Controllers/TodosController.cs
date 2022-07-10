using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RiskFirst.Hateoas;
using TodoApplication.Api.Extensions;
using TodoApplication.Api.Helpers;
using TodoApplication.Api.Models.Dtos;
using TodoApplication.Api.Models.Pagination;
using TodoApplication.Domain.Todo.Services.Interfaces;

namespace TodoApplication.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILinksService _linksService;

    private string Route => $"{Request.Scheme}://{Request.Host.ToUriComponent()}{Request.Path.Value}";
    
    public TodosController(ITodoService todoService, ILinksService linksService)
    {
        _todoService = todoService;
        _linksService = linksService;
    }

    [HttpGet("{id:long}", Name = "GetTodoItem")]
    [ProducesResponseType(typeof(TodoRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTodoItem(long id)
    {
        var todo = await _todoService.GetTodoAsync(id);

        var response = todo.MapToTodoResponseDto(GetTodoImageHref(todo.Id));
        
        await _linksService.AddLinksAsync(response);
        
        return StatusCode(StatusCodes.Status200OK, response);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PagedResource<TodoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllTodoItems([FromQuery]PaginationModel paginationModel)
    {
        var pagedTodos = await _todoService.GetAllTodosPagedAsync(paginationModel.PageNumber, paginationModel.PageSize);

        var pagedItems = pagedTodos.Items.Select(x => x.MapToTodoResponseDto(GetTodoImageHref(x.Id)))
            .ToList();
        
        await pagedItems.AddLinksAsync(_linksService);
        
        var response = PaginationHelper.CreatePagedResponse(pagedItems, Route, pagedTodos.TotalItemCount, pagedTodos.CurrentPageIndex, pagedTodos.PageSize);
        
        return StatusCode(StatusCodes.Status200OK, response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddTodo([FromForm] TodoRequestDto todoRequestDto)
    {
        var addedTodo = await _todoService.AddTodoAsync(todoRequestDto.Description, todoRequestDto.DueDate,
            todoRequestDto.Status, todoRequestDto.ImageFile?.FileName, todoRequestDto.ImageFile?.OpenReadStream());

        var response = addedTodo.MapToTodoResponseDto(GetTodoImageHref(addedTodo.Id));

        await _linksService.AddLinksAsync(response);
        
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("{todoId:long}/image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTodoImage(long todoId)
    {
        var todoImage = await _todoService.GetTodoImageByTodoIdAsync(todoId);
        
        return new PhysicalFileResult(todoImage.Path, MediaTypeNames.Image.Jpeg);
    }

    private string? GetTodoImageHref(long totoId)
    {
        return Url.Link("", new { Controller = "Todos", Action = "GetTodoImage", todoId = totoId });
    }
}
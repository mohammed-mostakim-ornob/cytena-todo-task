using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TodoApplication.Api.Validation.Attributes;
using TodoApplication.Domain.Todo.Models;

namespace TodoApplication.Api.Models.Dtos;

public class TodoRequestDto
{
    [Required]
    public string Description { get; set; }
    
    [Required]
    [DateTimeInFuture]
    public DateTime DueDate { get; set; }
    
    [Required]
    [DefaultValue(Status.Pending)]
    public Status Status { get; set; }
    
    [MaxFileSize(maxFileSizeInMb: 2)]
    [AllowedExtensions(new []{ "jpeg" })]
    public IFormFile? ImageFile { get; set; }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TodoApplication.Common.Exceptions;

namespace TodoApplication.Api.Filters;

public class CustomExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            ResourceNotFoundException => new NotFoundObjectResult(context.Exception.Message),
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError),
        };
    }
}
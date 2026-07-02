using FluentValidation;
using IdentityService.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Request failed with a handled API exception.");
            await WriteProblemDetailsAsync(context, ex);
        }
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, Exception exception)
    {
        var problem = exception switch
        {
            ValidationException validationException => new ProblemDetails
            {
                Title = "Validation failed",
                Detail = string.Join("; ", validationException.Errors.Select(error => error.ErrorMessage)),
                Status = StatusCodes.Status400BadRequest
            },
            InvalidOperationException => new ProblemDetails
            {
                Title = "Request conflict",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict
            },
            UnauthorizedException or UnauthorizedAccessException => new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = exception.Message,
                Status = StatusCodes.Status401Unauthorized
            },
            NotFoundException => new ProblemDetails
            {
                Title = "Resource not found",
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound
            },
            _ => new ProblemDetails
            {
                Title = "Unexpected error",
                Detail = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError
            }
        };

        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}

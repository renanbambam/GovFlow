using GovFlow.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FluentValidation.ValidationException;

namespace GovFlow.API.Middleware;

/// <summary>
/// Translates exceptions bubbling out of the MediatR pipeline / domain into RFC 7807
/// Problem Details responses. Keeps controllers free of try/catch noise.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            await WriteProblemAsync(context, exception);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case ValidationException validation:
                var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                await WriteAsync(context, new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred."
                });
                break;

            case NotFoundException notFound:
                await WriteAsync(context, Problem(StatusCodes.Status404NotFound, "Resource not found", notFound.Message));
                break;

            case ConflictException conflict:
                await WriteAsync(context, Problem(StatusCodes.Status409Conflict, "Conflict", conflict.Message));
                break;

            case UnauthorizedException unauthorized:
                await WriteAsync(context, Problem(StatusCodes.Status401Unauthorized, "Unauthorized", unauthorized.Message));
                break;

            case ArgumentException argument:
                await WriteAsync(context, Problem(StatusCodes.Status400BadRequest, "Invalid request", argument.Message));
                break;

            case InvalidOperationException invalid:
                await WriteAsync(context, Problem(StatusCodes.Status422UnprocessableEntity, "Operation not allowed", invalid.Message));
                break;

            default:
                _logger.LogError(exception, "Unhandled exception");
                await WriteAsync(context, Problem(StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred."));
                break;
        }
    }

    private static ProblemDetails Problem(int status, string title, string detail)
        => new() { Status = status, Title = title, Detail = detail };

    private static async Task WriteAsync(HttpContext context, ProblemDetails problem)
    {
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem, problem.GetType());
    }
}

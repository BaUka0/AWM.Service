using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using AWM.Service.Domain.Common;

namespace AWM.Service.WebAPI.Common.Middleware;

/// <summary>
/// Global exception handler that converts unhandled exceptions into ProblemDetails responses.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            DomainException de => (
                StatusCodes.Status422UnprocessableEntity,
                "Domain Error",
                de.Message),
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                string.Join("; ", validationException.Errors.Select(e => e.ErrorMessage))),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.")
        };

        if (exception is DomainException)
            _logger.LogWarning(exception, "Domain exception: {ErrorCode} - {Message}", 
                ((DomainException)exception).ErrorCode, exception.Message);
        else
            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Extensions = 
            {
                ["traceId"] = httpContext.TraceIdentifier,
                ["code"] = statusCode switch
                {
                    StatusCodes.Status422UnprocessableEntity => ((DomainException)exception).ErrorCode,
                    StatusCodes.Status400BadRequest => "ValidationError",
                    _ => "InternalError"
                }
            }
        };

        if (exception is ValidationException ve)
        {
            problemDetails.Extensions["validationErrors"] = ve.Errors.Select(e => new { 
                field = e.PropertyName, 
                message = e.ErrorMessage 
            });
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}

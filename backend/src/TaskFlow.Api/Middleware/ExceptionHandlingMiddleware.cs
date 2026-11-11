using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Exceptions;

namespace TaskFlow.Api.Middleware;

/// <summary>
/// Global exception handler that converts domain exceptions into RFC 7807 ProblemDetails responses.
/// 10-11-2026 Construido por: Ing GAAE - Centralized error handling for the whole API.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

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
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            BusinessRuleException => (StatusCodes.Status422UnprocessableEntity, "Regla de negocio incumplida"),
            ValidationException => (StatusCodes.Status400BadRequest, "Error de validación"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning("Handled exception ({StatusCode}): {Message}", statusCode, exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "Ocurrió un error inesperado. Inténtalo de nuevo más tarde."
                : exception.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Middleware;
using TaskFlow.Api.Repositories;
using TaskFlow.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 10-11-2026 Construido por: Ing GAAE - Application bootstrap and dependency registration.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // JSON responses use camelCase and enums are serialized as readable strings.
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Automatic model validation errors are returned in Spanish as ProblemDetails.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => LocalizeModelError(entry.Key, error.ErrorMessage))
                    .ToArray());

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Error de validación",
            Detail = "Revisa los datos enviados e inténtalo de nuevo.",
            Instance = context.HttpContext.Request.Path
        };

        return new BadRequestObjectResult(problemDetails);
    };
});

builder.Services.AddOpenApi();

// Database access through EF Core and SQL Server.
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency inversion: abstractions are registered against their implementations.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// CORS policy that allows the Angular development server to call the API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Global error handling and CORS must run before the controllers.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 10-11-2026 Construido por: Ing GAAE - ProblemDetails for empty error responses (404, 405, etc.).
app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    if (response.HasStarted)
    {
        return;
    }

    var (title, detail) = response.StatusCode switch
    {
        StatusCodes.Status404NotFound => ("Recurso no encontrado", "La ruta solicitada no existe."),
        StatusCodes.Status405MethodNotAllowed => ("Método no permitido", "El método HTTP no está permitido para esta ruta."),
        _ => ("Error en la solicitud", "No se pudo procesar la solicitud.")
    };

    var problemDetails = new ProblemDetails
    {
        Status = response.StatusCode,
        Title = title,
        Detail = detail,
        Instance = statusCodeContext.HttpContext.Request.Path
    };

    response.ContentType = "application/problem+json";
    await response.WriteAsync(JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
});

app.UseCors("AngularClient");

if (app.Environment.IsDevelopment())
{
    // OpenAPI document is exposed at /openapi/v1.json and the Scalar UI at /scalar/v1.
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();

// 10-11-2026 Construido por: Ing GAAE - Translates framework model errors to Spanish.
static string LocalizeModelError(string key, string message)
{
    if (key == "request" || string.IsNullOrEmpty(key) || message.Contains("request body", StringComparison.OrdinalIgnoreCase))
    {
        return "El cuerpo de la solicitud es obligatorio.";
    }

    if (key == "$" || key.StartsWith("$.", StringComparison.Ordinal))
    {
        return "El cuerpo de la solicitud no es un JSON válido.";
    }

    return string.IsNullOrWhiteSpace(message) ? "El valor enviado no es válido." : message;
}

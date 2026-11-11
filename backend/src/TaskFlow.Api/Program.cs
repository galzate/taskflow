using System.Text.Json;
using System.Text.Json.Serialization;
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
app.UseCors("AngularClient");

if (app.Environment.IsDevelopment())
{
    // OpenAPI document is exposed at /openapi/v1.json and the Scalar UI at /scalar/v1.
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();

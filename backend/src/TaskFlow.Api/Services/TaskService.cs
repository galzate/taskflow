using System.Text.Json;
using TaskFlow.Api.DTOs.Tasks;
using TaskFlow.Api.Entities;
using TaskFlow.Api.Enums;
using TaskFlow.Api.Exceptions;
using TaskFlow.Api.Repositories;

namespace TaskFlow.Api.Services;

/// <summary>
/// Business logic for task management, including the status transition rules.
/// 10-11-2026 Construido por: Ing GAAE - Enforces assignment, JSON metadata and status transition rules.
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAsync(
        int? userId,
        TaskItemStatus? status,
        string? sort,
        CancellationToken cancellationToken = default)
    {
        var ascending = string.Equals(sort, "asc", StringComparison.OrdinalIgnoreCase)
            || string.Equals(sort, "createdAt_asc", StringComparison.OrdinalIgnoreCase);

        var tasks = await _taskRepository.GetAsync(userId, status, ascending, cancellationToken);
        return tasks.Select(MapToResponse).ToList();
    }

    public async Task<TaskResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException($"No se encontró la tarea con id {id}.");
        }

        return MapToResponse(task);
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        // Business rule: a task must always be assigned to an existing user.
        var assignedUser = await _userRepository.GetByIdAsync(request.AssignedUserId, cancellationToken);
        if (assignedUser is null)
        {
            throw new NotFoundException($"No se encontró el usuario con id {request.AssignedUserId}.");
        }

        var metadataJson = NormalizeMetadata(request.Metadata);

        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Status = TaskItemStatus.Pending,
            AssignedUserId = request.AssignedUserId,
            Metadata = metadataJson,
            CreatedAt = DateTime.UtcNow
        };

        var createdTask = await _taskRepository.AddAsync(task, cancellationToken);
        createdTask.AssignedUser = assignedUser;
        return MapToResponse(createdTask);
    }

    public async Task<TaskResponse> UpdateStatusAsync(
        int id,
        UpdateTaskStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        // Defensive check: reject numeric values that are not part of the enum.
        if (!Enum.IsDefined(request.Status))
        {
            throw new ValidationException($"El estado '{request.Status}' no es válido.");
        }

        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException($"No se encontró la tarea con id {id}.");
        }

        // Idempotent update: requesting the current status is a no-op.
        if (task.Status == request.Status)
        {
            return MapToResponse(task);
        }

        // Business rule: a task cannot jump directly from Pending to Done.
        if (task.Status == TaskItemStatus.Pending && request.Status == TaskItemStatus.Done)
        {
            throw new BusinessRuleException(
                "Una tarea no puede pasar directamente de Pendiente a Completada. Primero muévela a En progreso.");
        }

        // Business rule: a Done task must be reopened through InProgress.
        if (task.Status == TaskItemStatus.Done && request.Status == TaskItemStatus.Pending)
        {
            throw new BusinessRuleException(
                "Una tarea completada no puede volver a Pendiente. Primero muévela a En progreso.");
        }

        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;
        await _taskRepository.UpdateAsync(task, cancellationToken);

        return MapToResponse(task);
    }

    /// <summary>
    /// Validates that the optional metadata is a JSON object and returns its raw JSON text.
    /// </summary>
    private static string? NormalizeMetadata(JsonElement? metadata)
    {
        if (!metadata.HasValue)
        {
            return null;
        }

        if (metadata.Value.ValueKind != JsonValueKind.Object)
        {
            throw new ValidationException("Los metadatos deben ser un objeto JSON.");
        }

        return metadata.Value.GetRawText();
    }

    private static TaskResponse MapToResponse(TaskItem task)
    {
        JsonElement? metadata = null;
        if (!string.IsNullOrWhiteSpace(task.Metadata))
        {
            using var document = JsonDocument.Parse(task.Metadata);
            metadata = document.RootElement.Clone();
        }

        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            AssignedUserId = task.AssignedUserId,
            AssignedUserName = task.AssignedUser?.Name ?? string.Empty,
            Metadata = metadata,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}

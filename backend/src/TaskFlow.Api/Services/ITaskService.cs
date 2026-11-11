using TaskFlow.Api.DTOs.Tasks;
using TaskFlow.Api.Enums;

namespace TaskFlow.Api.Services;

/// <summary>
/// Business operations for tasks. Implemented by <see cref="TaskService"/>.
/// 10-11-2026 Construido por: Ing GAAE - Task service abstraction.
/// </summary>
public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> GetAsync(
        int? userId,
        TaskItemStatus? status,
        string? sort,
        CancellationToken cancellationToken = default);

    Task<TaskResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);

    Task<TaskResponse> UpdateStatusAsync(
        int id,
        UpdateTaskStatusRequest request,
        CancellationToken cancellationToken = default);
}

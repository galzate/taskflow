using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Repositories;

/// <summary>
/// Data access contract for the task activity bitacora.
/// 10-11-2026 Construido por: Ing GAAE - Repository abstraction for TaskActivity.
/// </summary>
public interface ITaskActivityRepository
{
    Task AddAsync(TaskActivity entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskActivity>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken = default);
}

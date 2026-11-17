using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Repositories;

/// <summary>
/// Data access contract for the task status bitacora. Implemented by <see cref="TaskStatusHistoryRepository"/>.
/// 10-11-2026 Construido por: Ing GAAE - Repository abstraction for TaskStatusHistory.
/// </summary>
public interface ITaskStatusHistoryRepository
{
    Task AddAsync(TaskStatusHistory entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskStatusHistory>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken = default);
}

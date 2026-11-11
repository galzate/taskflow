using TaskFlow.Api.Entities;
using TaskFlow.Api.Enums;

namespace TaskFlow.Api.Repositories;

/// <summary>
/// Data access contract for tasks. Implemented by <see cref="TaskRepository"/>.
/// 10-11-2026 Construido por: Ing GAAE - Repository abstraction for the Tasks table.
/// </summary>
public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetAsync(
        int? userId,
        TaskItemStatus? status,
        bool ascending,
        CancellationToken cancellationToken = default);

    Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
}

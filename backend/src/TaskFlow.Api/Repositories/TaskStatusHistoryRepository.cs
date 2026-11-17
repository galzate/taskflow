using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ITaskStatusHistoryRepository"/>.
/// 10-11-2026 Construido por: Ing GAAE - Persists and queries the task status bitacora.
/// </summary>
public class TaskStatusHistoryRepository : ITaskStatusHistoryRepository
{
    private readonly TaskFlowDbContext _dbContext;

    public TaskStatusHistoryRepository(TaskFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TaskStatusHistory entry, CancellationToken cancellationToken = default)
    {
        _dbContext.TaskStatusHistory.Add(entry);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskStatusHistory>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaskStatusHistory
            .AsNoTracking()
            .Where(entry => entry.TaskId == taskId)
            .OrderBy(entry => entry.ChangedAt)
            .ToListAsync(cancellationToken);
    }
}

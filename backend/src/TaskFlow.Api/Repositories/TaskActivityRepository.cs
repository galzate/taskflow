using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ITaskActivityRepository"/>.
/// 10-11-2026 Construido por: Ing GAAE - Persists and queries the task activity bitacora.
/// </summary>
public class TaskActivityRepository : ITaskActivityRepository
{
    private readonly TaskFlowDbContext _dbContext;

    public TaskActivityRepository(TaskFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TaskActivity entry, CancellationToken cancellationToken = default)
    {
        _dbContext.TaskActivities.Add(entry);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskActivity>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaskActivities
            .AsNoTracking()
            .Where(entry => entry.TaskId == taskId)
            .OrderBy(entry => entry.LoggedAt)
            .ToListAsync(cancellationToken);
    }
}

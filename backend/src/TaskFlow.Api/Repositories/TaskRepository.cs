using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Entities;
using TaskFlow.Api.Enums;

namespace TaskFlow.Api.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ITaskRepository"/>.
/// 10-11-2026 Construido por: Ing GAAE - Filtered and sorted task queries through the DbContext.
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly TaskFlowDbContext _dbContext;

    public TaskRepository(TaskFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAsync(
        int? userId,
        TaskItemStatus? status,
        bool ascending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = _dbContext.Tasks
            .AsNoTracking()
            .Include(task => task.AssignedUser);

        if (userId.HasValue)
        {
            query = query.Where(task => task.AssignedUserId == userId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(task => task.Status == status.Value);
        }

        query = ascending
            ? query.OrderBy(task => task.CreatedAt)
            : query.OrderByDescending(task => task.CreatedAt);

        return await query.ToListAsync(cancellationToken);
    }

    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Tasks
            .Include(task => task.AssignedUser)
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
    }

    public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        _dbContext.Tasks.Update(task);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

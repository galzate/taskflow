using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Data;

/// <summary>
/// EF Core database context for the task management system.
/// 10-11-2026 Construido por: Ing GAAE - DbContext exposing the Users and Tasks sets.
/// </summary>
public class TaskFlowDbContext : DbContext
{
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options)
    {
    }

    /// <summary>Users registered in the system.</summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>Tasks tracked by the system.</summary>
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 10-11-2026 Construido por: Ing GAAE - Applies all IEntityTypeConfiguration mappings.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskFlowDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

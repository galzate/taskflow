using TaskFlow.Api.Enums;

namespace TaskFlow.Api.Entities;

/// <summary>
/// Audit log entry that records every status change of a task.
/// 10-11-2026 Construido por: Ing GAAE - Bitacora entity for task status history.
/// </summary>
public class TaskStatusHistory
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Foreign key to the task whose status changed.</summary>
    public int TaskId { get; set; }

    /// <summary>Navigation property to the task.</summary>
    public TaskItem? Task { get; set; }

    /// <summary>Status before the change.</summary>
    public TaskItemStatus FromStatus { get; set; }

    /// <summary>Status after the change.</summary>
    public TaskItemStatus ToStatus { get; set; }

    /// <summary>Optional observation captured when the change happened.</summary>
    public string? Observation { get; set; }

    /// <summary>UTC timestamp of the change.</summary>
    public DateTime ChangedAt { get; set; }
}

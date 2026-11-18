namespace TaskFlow.Api.Entities;

/// <summary>
/// Activity log entry that records what was done on a task.
/// 10-11-2026 Construido por: Ing GAAE - Activity bitacora entity.
/// </summary>
public class TaskActivity
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Foreign key to the task.</summary>
    public int TaskId { get; set; }

    /// <summary>Navigation property to the task.</summary>
    public TaskItem? Task { get; set; }

    /// <summary>Description of what was done.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the activity was logged.</summary>
    public DateTime LoggedAt { get; set; }
}

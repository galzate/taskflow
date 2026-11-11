using TaskFlow.Api.Enums;

namespace TaskFlow.Api.Entities;

/// <summary>
/// Represents a task assigned to a user.
/// 10-11-2026 Construido por: Ing GAAE - Task entity mapped to the Tasks table.
/// </summary>
public class TaskItem
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Short title that describes the task. Required.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional detailed description of the task.</summary>
    public string? Description { get; set; }

    /// <summary>Current lifecycle status of the task.</summary>
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

    /// <summary>Foreign key to the assigned user. Required.</summary>
    public int AssignedUserId { get; set; }

    /// <summary>Navigation property to the assigned user.</summary>
    public User? AssignedUser { get; set; }

    /// <summary>Optional JSON payload with extra task attributes (priority, tags, due date, etc.).</summary>
    public string? Metadata { get; set; }

    /// <summary>UTC timestamp when the task was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>UTC timestamp of the last status update.</summary>
    public DateTime? UpdatedAt { get; set; }
}

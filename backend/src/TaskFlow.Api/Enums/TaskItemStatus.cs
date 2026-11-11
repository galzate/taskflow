namespace TaskFlow.Api.Enums;

/// <summary>
/// Represents the lifecycle states allowed for a task.
/// 10-11-2026 Construido por: Ing GAAE - Defines the allowed task statuses (Pending, InProgress, Done).
/// </summary>
public enum TaskItemStatus
{
    /// <summary>The task has been created but work has not started yet.</summary>
    Pending = 0,

    /// <summary>The task is currently being worked on.</summary>
    InProgress = 1,

    /// <summary>The task has been completed.</summary>
    Done = 2
}

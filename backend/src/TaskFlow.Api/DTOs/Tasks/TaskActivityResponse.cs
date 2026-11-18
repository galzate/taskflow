namespace TaskFlow.Api.DTOs.Tasks;

/// <summary>
/// Activity log entry returned for a task.
/// 10-11-2026 Construido por: Ing GAAE - Activity bitacora response DTO.
/// </summary>
public class TaskActivityResponse
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Description of what was done.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the activity was logged.</summary>
    public DateTime LoggedAt { get; set; }
}

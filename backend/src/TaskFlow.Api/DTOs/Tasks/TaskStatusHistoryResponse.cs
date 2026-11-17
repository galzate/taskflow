using TaskFlow.Api.Enums;

namespace TaskFlow.Api.DTOs.Tasks;

/// <summary>
/// Audit log entry returned for a task status change.
/// 10-11-2026 Construido por: Ing GAAE - Bitacora response DTO.
/// </summary>
public class TaskStatusHistoryResponse
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Status before the change.</summary>
    public TaskItemStatus FromStatus { get; set; }

    /// <summary>Status after the change.</summary>
    public TaskItemStatus ToStatus { get; set; }

    /// <summary>Optional observation captured when the change happened.</summary>
    public string? Observation { get; set; }

    /// <summary>UTC timestamp of the change.</summary>
    public DateTime ChangedAt { get; set; }
}

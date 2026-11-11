using System.ComponentModel.DataAnnotations;
using TaskFlow.Api.Enums;

namespace TaskFlow.Api.DTOs.Tasks;

/// <summary>
/// Payload used to change the status of an existing task.
/// 10-11-2026 Construido por: Ing GAAE - Input DTO for task status transitions.
/// </summary>
public class UpdateTaskStatusRequest
{
    /// <summary>Target status: Pending, InProgress or Done.</summary>
    [Required(ErrorMessage = "El estado es obligatorio.")]
    [EnumDataType(typeof(TaskItemStatus), ErrorMessage = "El estado debe ser uno de: Pending, InProgress, Done.")]
    public TaskItemStatus Status { get; set; }
}

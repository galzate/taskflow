using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TaskFlow.Api.Enums;

namespace TaskFlow.Api.DTOs.Tasks;

/// <summary>
/// Payload used to update an existing task's editable fields.
/// 10-11-2026 Construido por: Ing GAAE - Input DTO for task edition.
/// </summary>
public class UpdateTaskRequest
{
    /// <summary>Short title that describes the task. Required.</summary>
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Detailed description of the task. Required.</summary>
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Foreign key to the assigned user. Required.</summary>
    [Required(ErrorMessage = "Debes seleccionar un usuario asignado.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un usuario asignado.")]
    public int AssignedUserId { get; set; }

    /// <summary>Optional JSON payload with extra task attributes (priority, tags, due date, etc.).</summary>
    public JsonElement? Metadata { get; set; }
}

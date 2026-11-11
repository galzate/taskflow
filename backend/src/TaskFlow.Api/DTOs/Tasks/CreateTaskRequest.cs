using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace TaskFlow.Api.DTOs.Tasks;

/// <summary>
/// Payload used to create a new task.
/// 10-11-2026 Construido por: Ing GAAE - Input DTO for task creation with validation rules.
/// </summary>
public class CreateTaskRequest
{
    /// <summary>Short title that describes the task. Required.</summary>
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "El título debe tener entre 1 y 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional detailed description of the task.</summary>
    [StringLength(1000, ErrorMessage = "La descripción no debe superar los 1000 caracteres.")]
    public string? Description { get; set; }

    /// <summary>Identifier of the user the task is assigned to. Required.</summary>
    [Required(ErrorMessage = "El usuario asignado es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El usuario asignado debe ser un identificador válido.")]
    public int AssignedUserId { get; set; }

    /// <summary>Optional JSON object with extra task attributes (priority, tags, due date, etc.).</summary>
    public JsonElement? Metadata { get; set; }
}

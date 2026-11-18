using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.DTOs.Tasks;

/// <summary>
/// Payload used to register an activity in the task bitacora.
/// 10-11-2026 Construido por: Ing GAAE - Input DTO for task activity log.
/// </summary>
public class CreateTaskActivityRequest
{
    /// <summary>Description of what was done on the task.</summary>
    [Required(ErrorMessage = "La descripción de la actividad es obligatoria.")]
    [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
    public string Description { get; set; } = string.Empty;
}

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
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional detailed description of the task.</summary>
    [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
    public string? Description { get; set; }

    /// <summary>Identifier of the user the task is assigned to. Required.</summary>
    [Required(ErrorMessage = "AssignedUserId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "AssignedUserId must be a positive integer.")]
    public int AssignedUserId { get; set; }

    /// <summary>Optional JSON object with extra task attributes (priority, tags, due date, etc.).</summary>
    public JsonElement? Metadata { get; set; }
}

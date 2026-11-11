using System.Text.Json;
using TaskFlow.Api.Enums;

namespace TaskFlow.Api.DTOs.Tasks;

/// <summary>
/// Output DTO that represents a task returned by the API.
/// 10-11-2026 Construido por: Ing GAAE - Response DTO for task data including the assigned user name.
/// </summary>
public class TaskResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; }
    public int AssignedUserId { get; set; }
    public string AssignedUserName { get; set; } = string.Empty;
    public JsonElement? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

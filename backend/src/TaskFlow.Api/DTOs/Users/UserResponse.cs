namespace TaskFlow.Api.DTOs.Users;

/// <summary>
/// Output DTO that represents a user returned by the API.
/// 10-11-2026 Construido por: Ing GAAE - Response DTO for user data.
/// </summary>
public class UserResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

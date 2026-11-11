using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.DTOs.Users;

/// <summary>
/// Payload used to create a new user.
/// 10-11-2026 Construido por: Ing GAAE - Input DTO for user creation with validation rules.
/// </summary>
public class CreateUserRequest
{
    /// <summary>Full name of the user.</summary>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Unique email address of the user.</summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    [StringLength(150, ErrorMessage = "Email must not exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;
}

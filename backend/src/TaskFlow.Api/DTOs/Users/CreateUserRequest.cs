using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.DTOs.Users;

/// <summary>
/// Payload used to create a new user.
/// 10-11-2026 Construido por: Ing GAAE - Input DTO for user creation with validation rules.
/// </summary>
public class CreateUserRequest
{
    /// <summary>Full name of the user.</summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Unique email address of the user.</summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [StringLength(150, ErrorMessage = "El correo electrónico no debe superar los 150 caracteres.")]
    public string Email { get; set; } = string.Empty;
}

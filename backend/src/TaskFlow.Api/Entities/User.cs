namespace TaskFlow.Api.Entities;

/// <summary>
/// Represents an application user that can own tasks.
/// 10-11-2026 Construido por: Ing GAAE - User entity mapped to the Users table.
/// </summary>
public class User
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Full name of the user.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Unique email address of the user.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the user was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Tasks assigned to this user.</summary>
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}

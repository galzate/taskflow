namespace TaskFlow.Api.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist. Maps to HTTP 404.
/// 10-11-2026 Construido por: Ing GAAE - Not found exception for missing resources.
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message)
    {
    }
}

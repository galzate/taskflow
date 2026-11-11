namespace TaskFlow.Api.Exceptions;

/// <summary>
/// Thrown when input data is semantically invalid. Maps to HTTP 400.
/// 10-11-2026 Construido por: Ing GAAE - Validation exception for invalid input payloads.
/// </summary>
public class ValidationException : AppException
{
    public ValidationException(string message) : base(message)
    {
    }
}

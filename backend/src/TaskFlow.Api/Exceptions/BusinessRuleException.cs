namespace TaskFlow.Api.Exceptions;

/// <summary>
/// Thrown when a business rule is violated. Maps to HTTP 422.
/// 10-11-2026 Construido por: Ing GAAE - Business rule exception (e.g., invalid status transition).
/// </summary>
public class BusinessRuleException : AppException
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}

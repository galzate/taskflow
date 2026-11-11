namespace TaskFlow.Api.Exceptions;

/// <summary>
/// Base class for all expected application exceptions.
/// 10-11-2026 Construido por: Ing GAAE - Common base exception for controlled errors.
/// </summary>
public abstract class AppException : Exception
{
    protected AppException(string message) : base(message)
    {
    }
}

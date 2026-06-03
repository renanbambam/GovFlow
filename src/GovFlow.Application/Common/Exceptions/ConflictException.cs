namespace GovFlow.Application.Common.Exceptions;

/// <summary>
/// Thrown by a handler when a command conflicts with the current state (e.g. a unique
/// constraint). Mapped to HTTP 409 by the API layer.
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}

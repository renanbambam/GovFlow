namespace GovFlow.Application.Common.Exceptions;

/// <summary>
/// Thrown when credentials are invalid or a token cannot be honoured. Mapped to HTTP 401.
/// </summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}

namespace GovFlow.Application.Common.Exceptions;

/// <summary>
/// Thrown by a handler when an aggregate referenced by a command cannot be found.
/// Mapped to HTTP 404 by the API layer.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException For(string entity, Guid id)
        => new($"{entity} '{id}' was not found.");
}

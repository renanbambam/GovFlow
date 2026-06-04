namespace GovFlow.Application.Common.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException For(string entity, Guid id)
        => new($"{entity} '{id}' was not found.");
}

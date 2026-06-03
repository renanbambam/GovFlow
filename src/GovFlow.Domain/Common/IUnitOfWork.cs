namespace GovFlow.Domain.Common;

/// <summary>
/// Commits the changes tracked across one or more repositories as a single unit.
/// Implemented by the persistence layer (the EF Core DbContext).
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

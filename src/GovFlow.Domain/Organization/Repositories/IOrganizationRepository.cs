namespace GovFlow.Domain.Organization;

/// <summary>Persistence contract for the <see cref="Organization"/> aggregate.</summary>
public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);

    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);
}

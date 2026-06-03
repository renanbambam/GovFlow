using GovFlow.Domain.Organization;
using Microsoft.EntityFrameworkCore;
using OrganizationAggregate = GovFlow.Domain.Organization.Organization;

namespace GovFlow.Infrastructure.Persistence.Repositories;

internal sealed class OrganizationRepository : IOrganizationRepository
{
    private readonly GovFlowDbContext _context;

    public OrganizationRepository(GovFlowDbContext context) => _context = context;

    public Task<OrganizationAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Organizations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
        => _context.Organizations.AnyAsync(x => x.Slug == slug, cancellationToken);

    public async Task AddAsync(OrganizationAggregate organization, CancellationToken cancellationToken = default)
        => await _context.Organizations.AddAsync(organization, cancellationToken);
}

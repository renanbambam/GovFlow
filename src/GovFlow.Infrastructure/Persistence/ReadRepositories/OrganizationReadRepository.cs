using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Organization.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.ReadRepositories;

internal sealed class OrganizationReadRepository : IOrganizationReadRepository
{
    private readonly GovFlowDbContext _context;

    public OrganizationReadRepository(GovFlowDbContext context) => _context = context;

    public async Task<OrganizationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Organizations
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrganizationDto(o.Id, o.Name, o.Slug, o.IsActive, o.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<OrganizationDto>> ListAsync(CancellationToken cancellationToken = default)
        => await _context.Organizations
            .AsNoTracking()
            .OrderBy(o => o.Name)
            .Select(o => new OrganizationDto(o.Id, o.Name, o.Slug, o.IsActive, o.CreatedAt))
            .ToListAsync(cancellationToken);
}

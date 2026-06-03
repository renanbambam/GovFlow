using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using GovFlow.Domain.Process;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.ReadRepositories;

internal sealed class ProcessTypeReadRepository : IProcessTypeReadRepository
{
    private readonly GovFlowDbContext _context;

    public ProcessTypeReadRepository(GovFlowDbContext context) => _context = context;

    public async Task<ProcessTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProcessTypes
            .AsNoTracking()
            .Include(p => p.Steps)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<ProcessTypeDto>> ListAsync(Guid? organizationId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.ProcessTypes
            .AsNoTracking()
            .Include(p => p.Steps)
            .Where(p => organizationId == null || p.OrganizationId == organizationId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDto).ToList();
    }

    private static ProcessTypeDto ToDto(ProcessType entity)
        => new(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.OrganizationId,
            entity.IsActive,
            entity.CreatedAt,
            entity.Steps
                .OrderBy(s => s.Order)
                .Select(s => new WorkflowStepDto(
                    s.Id,
                    s.Name,
                    s.Description,
                    s.Order,
                    s.AssignableDepartmentId,
                    s.SlaHours,
                    s.RequiredDocuments.ToList()))
                .ToList());
}

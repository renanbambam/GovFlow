using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using GovFlow.Domain.Process;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.ReadRepositories;

internal sealed class ProcessReadRepository : IProcessReadRepository
{
    private readonly GovFlowDbContext _context;

    public ProcessReadRepository(GovFlowDbContext context) => _context = context;

    public async Task<ProcessInstanceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProcessInstances
            .AsNoTracking()
            .Include(p => p.Steps)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<ProcessSummaryDto>> ListAsync(Guid? organizationId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.ProcessInstances
            .AsNoTracking()
            .Where(p => organizationId == null || p.OrganizationId == organizationId)
            .OrderByDescending(p => p.OpenedAt)
            .Select(p => new
            {
                p.Id,
                p.ProcessTypeId,
                p.OrganizationId,
                p.Title,
                p.Status,
                p.Priority,
                p.OpenedAt,
                p.ClosedAt
            })
            .ToListAsync(cancellationToken);

        return rows
            .Select(r => new ProcessSummaryDto(
                r.Id,
                r.ProcessTypeId,
                r.OrganizationId,
                r.Title,
                r.Status.ToString(),
                r.Priority.ToString(),
                r.OpenedAt,
                r.ClosedAt))
            .ToList();
    }

    public async Task<IReadOnlyList<ProcessTimelineEntryDto>?> GetTimelineAsync(Guid processId, CancellationToken cancellationToken = default)
    {
        var exists = await _context.ProcessInstances
            .AsNoTracking()
            .AnyAsync(p => p.Id == processId, cancellationToken);
        if (!exists)
            return null;

        var entries = await _context.ProcessTimelineEntries
            .AsNoTracking()
            .Where(e => e.ProcessInstanceId == processId)
            .OrderBy(e => e.OccurredAt)
            .ThenBy(e => e.Sequence)
            .Select(e => new ProcessTimelineEntryDto(
                e.Id,
                e.Sequence,
                e.EventType.ToString(),
                e.Description,
                e.StepId,
                e.OccurredAt))
            .ToListAsync(cancellationToken);

        return entries;
    }

    private static ProcessInstanceDto ToDto(ProcessInstance entity)
        => new(
            entity.Id,
            entity.ProcessTypeId,
            entity.OrganizationId,
            entity.Title,
            entity.Description,
            entity.RequesterId,
            entity.CurrentStepId,
            entity.Status.ToString(),
            entity.Priority.ToString(),
            entity.OpenedAt,
            entity.ClosedAt,
            entity.DueAt,
            entity.Steps
                .OrderBy(s => s.Sequence)
                .Select(s => new ProcessInstanceStepDto(
                    s.Id,
                    s.WorkflowStepId,
                    s.Sequence,
                    s.Status.ToString(),
                    s.AssignedUserId,
                    s.AssignedDepartmentId,
                    s.StartedAt,
                    s.CompletedAt,
                    s.Notes))
                .ToList());
}

using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Dashboard.Dtos;
using GovFlow.Domain.Process.Enums;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.ReadRepositories;

internal sealed class DashboardReadRepository : IDashboardReadRepository
{
    private static readonly ProcessStatus[] OpenStatuses =
        { ProcessStatus.Draft, ProcessStatus.Open, ProcessStatus.InProgress, ProcessStatus.OnHold };

    private readonly GovFlowDbContext _context;

    public DashboardReadRepository(GovFlowDbContext context) => _context = context;

    public async Task<DashboardDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var totalOrganizations = await _context.Organizations.CountAsync(cancellationToken);
        var totalProcessTypes = await _context.ProcessTypes.CountAsync(cancellationToken);
        var totalProcesses = await _context.ProcessInstances.CountAsync(cancellationToken);
        var totalOpen = await _context.ProcessInstances
            .CountAsync(p => OpenStatuses.Contains(p.Status), cancellationToken);
        var totalCompleted = await _context.ProcessInstances
            .CountAsync(p => p.Status == ProcessStatus.Resolved, cancellationToken);

        return new DashboardDto(
            totalOrganizations,
            totalProcessTypes,
            totalOpen,
            totalCompleted,
            totalProcesses);
    }
}

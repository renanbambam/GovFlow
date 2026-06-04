using GovFlow.Domain.Process;
using GovFlow.Domain.Process.Enums;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.Repositories;

internal sealed class ProcessInstanceRepository : IProcessInstanceRepository
{
    private readonly GovFlowDbContext _context;

    public ProcessInstanceRepository(GovFlowDbContext context) => _context = context;

    public Task<ProcessInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.ProcessInstances
            .Include(x => x.Steps)
            .Include(x => x.Timeline)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.ProcessInstances.AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Guid>> GetStalledProcessIdsAsync(DateTime threshold, CancellationToken cancellationToken = default)
        => await _context.ProcessInstances
            .Where(p => p.Status == ProcessStatus.Open
                     || p.Status == ProcessStatus.InProgress
                     || p.Status == ProcessStatus.OnHold)
            .Where(p => !p.Timeline.Any(e => e.OccurredAt >= threshold))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ProcessInstance instance, CancellationToken cancellationToken = default)
        => await _context.ProcessInstances.AddAsync(instance, cancellationToken);
}

using GovFlow.Domain.Process;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.Repositories;

internal sealed class ProcessInstanceRepository : IProcessInstanceRepository
{
    private readonly GovFlowDbContext _context;

    public ProcessInstanceRepository(GovFlowDbContext context) => _context = context;

    public Task<ProcessInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.ProcessInstances
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(ProcessInstance instance, CancellationToken cancellationToken = default)
        => await _context.ProcessInstances.AddAsync(instance, cancellationToken);
}

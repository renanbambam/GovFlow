using GovFlow.Domain.Process;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.Repositories;

internal sealed class ProcessTypeRepository : IProcessTypeRepository
{
    private readonly GovFlowDbContext _context;

    public ProcessTypeRepository(GovFlowDbContext context) => _context = context;

    public Task<ProcessType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.ProcessTypes
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(ProcessType processType, CancellationToken cancellationToken = default)
        => await _context.ProcessTypes.AddAsync(processType, cancellationToken);
}

using GovFlow.Domain.Process;

namespace GovFlow.Infrastructure.Persistence.Repositories;

internal sealed class ProcessCommentRepository : IProcessCommentRepository
{
    private readonly GovFlowDbContext _context;

    public ProcessCommentRepository(GovFlowDbContext context) => _context = context;

    public async Task AddAsync(ProcessComment comment, CancellationToken cancellationToken = default)
        => await _context.ProcessComments.AddAsync(comment, cancellationToken);
}

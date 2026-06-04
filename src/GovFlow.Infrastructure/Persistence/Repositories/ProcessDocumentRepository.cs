using GovFlow.Domain.Process;

namespace GovFlow.Infrastructure.Persistence.Repositories;

internal sealed class ProcessDocumentRepository : IProcessDocumentRepository
{
    private readonly GovFlowDbContext _context;

    public ProcessDocumentRepository(GovFlowDbContext context) => _context = context;

    public async Task AddAsync(ProcessDocument document, CancellationToken cancellationToken = default)
        => await _context.ProcessDocuments.AddAsync(document, cancellationToken);
}

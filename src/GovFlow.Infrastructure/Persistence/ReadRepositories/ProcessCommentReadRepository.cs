using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.ReadRepositories;

internal sealed class ProcessCommentReadRepository : IProcessCommentReadRepository
{
    private readonly GovFlowDbContext _context;

    public ProcessCommentReadRepository(GovFlowDbContext context) => _context = context;

    public async Task<IReadOnlyList<ProcessCommentDto>?> ListByProcessAsync(Guid processId, CancellationToken cancellationToken = default)
    {
        var exists = await _context.ProcessInstances
            .AsNoTracking()
            .AnyAsync(p => p.Id == processId, cancellationToken);
        if (!exists)
            return null;

        return await _context.ProcessComments
            .AsNoTracking()
            .Where(c => c.ProcessInstanceId == processId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new ProcessCommentDto(c.Id, c.ProcessInstanceId, c.AuthorId, c.Content, c.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}

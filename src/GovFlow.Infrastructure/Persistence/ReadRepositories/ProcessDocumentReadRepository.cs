using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.ReadRepositories;

internal sealed class ProcessDocumentReadRepository : IProcessDocumentReadRepository
{
    private readonly GovFlowDbContext _context;

    public ProcessDocumentReadRepository(GovFlowDbContext context) => _context = context;

    public async Task<IReadOnlyList<ProcessDocumentDto>?> ListByProcessAsync(Guid processId, CancellationToken cancellationToken = default)
    {
        var exists = await _context.ProcessInstances
            .AsNoTracking()
            .AnyAsync(p => p.Id == processId, cancellationToken);
        if (!exists)
            return null;

        return await _context.ProcessDocuments
            .AsNoTracking()
            .Where(d => d.ProcessInstanceId == processId)
            .OrderByDescending(d => d.UploadedAt)
            .Select(d => new ProcessDocumentDto(
                d.Id,
                d.ProcessInstanceId,
                d.UploadedByUserId,
                d.FileName,
                d.ContentType,
                d.SizeBytes,
                d.UploadedAt))
            .ToListAsync(cancellationToken);
    }
}

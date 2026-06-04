namespace GovFlow.Domain.Process;

public interface IProcessDocumentRepository
{
    Task AddAsync(ProcessDocument document, CancellationToken cancellationToken = default);
}

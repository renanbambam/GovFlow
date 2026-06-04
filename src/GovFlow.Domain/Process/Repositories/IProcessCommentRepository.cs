namespace GovFlow.Domain.Process;

public interface IProcessCommentRepository
{
    Task AddAsync(ProcessComment comment, CancellationToken cancellationToken = default);
}

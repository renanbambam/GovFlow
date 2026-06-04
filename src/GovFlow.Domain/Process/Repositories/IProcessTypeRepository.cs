namespace GovFlow.Domain.Process;

public interface IProcessTypeRepository
{
    Task<ProcessType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(ProcessType processType, CancellationToken cancellationToken = default);
}

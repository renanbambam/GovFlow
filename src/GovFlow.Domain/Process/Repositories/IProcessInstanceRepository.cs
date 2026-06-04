namespace GovFlow.Domain.Process;

public interface IProcessInstanceRepository
{
    Task<ProcessInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> GetStalledProcessIdsAsync(DateTime threshold, CancellationToken cancellationToken = default);

    Task AddAsync(ProcessInstance instance, CancellationToken cancellationToken = default);
}

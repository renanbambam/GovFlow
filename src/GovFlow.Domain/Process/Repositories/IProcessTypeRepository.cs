namespace GovFlow.Domain.Process;

/// <summary>Persistence contract for the <see cref="ProcessType"/> aggregate (with its steps).</summary>
public interface IProcessTypeRepository
{
    /// <summary>Loads a process type including its ordered <see cref="WorkflowStep"/> collection.</summary>
    Task<ProcessType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(ProcessType processType, CancellationToken cancellationToken = default);
}

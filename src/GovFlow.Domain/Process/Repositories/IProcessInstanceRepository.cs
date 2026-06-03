namespace GovFlow.Domain.Process;

/// <summary>Persistence contract for the <see cref="ProcessInstance"/> aggregate (with its steps).</summary>
public interface IProcessInstanceRepository
{
    /// <summary>Loads a process instance including its <see cref="ProcessInstanceStep"/> collection.</summary>
    Task<ProcessInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(ProcessInstance instance, CancellationToken cancellationToken = default);
}

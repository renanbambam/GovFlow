namespace GovFlow.Domain.Organization;

/// <summary>Persistence contract for the <see cref="Department"/> aggregate.</summary>
public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Department department, CancellationToken cancellationToken = default);
}

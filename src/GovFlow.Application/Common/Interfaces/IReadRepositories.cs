using GovFlow.Application.Dashboard.Dtos;
using GovFlow.Application.Organization.Dtos;
using GovFlow.Application.Process.Dtos;

namespace GovFlow.Application.Common.Interfaces;

/// <summary>
/// Read-side contracts (CQRS query side). Implementations live in Infrastructure and project
/// straight to DTOs with EF Core, never exposing domain entities to the Application handlers.
/// </summary>
public interface IOrganizationReadRepository
{
    Task<OrganizationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrganizationDto>> ListAsync(CancellationToken cancellationToken = default);
}

public interface IProcessTypeReadRepository
{
    Task<ProcessTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProcessTypeDto>> ListAsync(Guid? organizationId, CancellationToken cancellationToken = default);
}

public interface IProcessReadRepository
{
    Task<ProcessInstanceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProcessSummaryDto>> ListAsync(Guid? organizationId, CancellationToken cancellationToken = default);
}

public interface IDashboardReadRepository
{
    Task<DashboardDto> GetAsync(CancellationToken cancellationToken = default);
}

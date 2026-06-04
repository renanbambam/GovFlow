using GovFlow.Application.Dashboard.Dtos;
using GovFlow.Application.Organization.Dtos;
using GovFlow.Application.Process.Dtos;

namespace GovFlow.Application.Common.Interfaces;

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

    Task<IReadOnlyList<ProcessTimelineEntryDto>?> GetTimelineAsync(Guid processId, CancellationToken cancellationToken = default);
}

public interface IProcessCommentReadRepository
{
    Task<IReadOnlyList<ProcessCommentDto>?> ListByProcessAsync(Guid processId, CancellationToken cancellationToken = default);
}

public interface IProcessDocumentReadRepository
{
    Task<IReadOnlyList<ProcessDocumentDto>?> ListByProcessAsync(Guid processId, CancellationToken cancellationToken = default);
}

public interface IDashboardReadRepository
{
    Task<DashboardDto> GetAsync(CancellationToken cancellationToken = default);
}

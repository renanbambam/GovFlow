using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using MediatR;

namespace GovFlow.Application.Process.Queries;

/// <summary>Lists process instances (summary view), optionally filtered by organization.</summary>
public sealed record GetProcessesQuery(Guid? OrganizationId = null) : IRequest<IReadOnlyList<ProcessSummaryDto>>;

public sealed class GetProcessesQueryHandler : IRequestHandler<GetProcessesQuery, IReadOnlyList<ProcessSummaryDto>>
{
    private readonly IProcessReadRepository _processes;

    public GetProcessesQueryHandler(IProcessReadRepository processes) => _processes = processes;

    public Task<IReadOnlyList<ProcessSummaryDto>> Handle(GetProcessesQuery request, CancellationToken cancellationToken)
        => _processes.ListAsync(request.OrganizationId, cancellationToken);
}

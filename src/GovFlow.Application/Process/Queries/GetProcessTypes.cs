using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using MediatR;

namespace GovFlow.Application.Process.Queries;

public sealed record GetProcessTypesQuery(Guid? OrganizationId = null) : IRequest<IReadOnlyList<ProcessTypeDto>>;

public sealed class GetProcessTypesQueryHandler
    : IRequestHandler<GetProcessTypesQuery, IReadOnlyList<ProcessTypeDto>>
{
    private readonly IProcessTypeReadRepository _processTypes;

    public GetProcessTypesQueryHandler(IProcessTypeReadRepository processTypes) => _processTypes = processTypes;

    public Task<IReadOnlyList<ProcessTypeDto>> Handle(GetProcessTypesQuery request, CancellationToken cancellationToken)
        => _processTypes.ListAsync(request.OrganizationId, cancellationToken);
}

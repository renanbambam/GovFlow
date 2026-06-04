using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Organization.Dtos;
using MediatR;

namespace GovFlow.Application.Organization.Queries;

public sealed record GetOrganizationsQuery : IRequest<IReadOnlyList<OrganizationDto>>;

public sealed class GetOrganizationsQueryHandler
    : IRequestHandler<GetOrganizationsQuery, IReadOnlyList<OrganizationDto>>
{
    private readonly IOrganizationReadRepository _organizations;

    public GetOrganizationsQueryHandler(IOrganizationReadRepository organizations) => _organizations = organizations;

    public Task<IReadOnlyList<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
        => _organizations.ListAsync(cancellationToken);
}

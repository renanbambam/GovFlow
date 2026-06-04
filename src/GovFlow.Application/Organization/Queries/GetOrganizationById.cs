using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Organization.Dtos;
using MediatR;

namespace GovFlow.Application.Organization.Queries;

public sealed record GetOrganizationByIdQuery(Guid Id) : IRequest<OrganizationDto>;

public sealed class GetOrganizationByIdQueryHandler : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto>
{
    private readonly IOrganizationReadRepository _organizations;

    public GetOrganizationByIdQueryHandler(IOrganizationReadRepository organizations) => _organizations = organizations;

    public async Task<OrganizationDto> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
        => await _organizations.GetByIdAsync(request.Id, cancellationToken)
           ?? throw NotFoundException.For("Organization", request.Id);
}

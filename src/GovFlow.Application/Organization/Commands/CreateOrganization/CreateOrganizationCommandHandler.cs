using GovFlow.Application.Common.Exceptions;
using GovFlow.Domain.Common;
using GovFlow.Domain.Organization;
using MediatR;
using OrganizationAggregate = GovFlow.Domain.Organization.Organization;

namespace GovFlow.Application.Organization.Commands.CreateOrganization;

public sealed class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, Guid>
{
    private readonly IOrganizationRepository _organizations;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrganizationCommandHandler(IOrganizationRepository organizations, IUnitOfWork unitOfWork)
    {
        _organizations = organizations;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (await _organizations.SlugExistsAsync(slug, cancellationToken))
            throw new ConflictException($"An organization with slug '{slug}' already exists.");

        var organization = OrganizationAggregate.Create(request.Name, slug);

        await _organizations.AddAsync(organization, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return organization.Id;
    }
}

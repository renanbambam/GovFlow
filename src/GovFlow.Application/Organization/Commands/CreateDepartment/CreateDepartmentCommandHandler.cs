using GovFlow.Application.Common.Exceptions;
using GovFlow.Domain.Common;
using GovFlow.Domain.Organization;
using MediatR;

namespace GovFlow.Application.Organization.Commands.CreateDepartment;

public sealed class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
{
    private readonly IOrganizationRepository _organizations;
    private readonly IDepartmentRepository _departments;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentCommandHandler(
        IOrganizationRepository organizations,
        IDepartmentRepository departments,
        IUnitOfWork unitOfWork)
    {
        _organizations = organizations;
        _departments = departments;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        if (await _organizations.GetByIdAsync(request.OrganizationId, cancellationToken) is null)
            throw NotFoundException.For("Organization", request.OrganizationId);

        if (request.ParentDepartmentId is { } parentId)
        {
            var parent = await _departments.GetByIdAsync(parentId, cancellationToken)
                ?? throw NotFoundException.For("Department", parentId);

            if (parent.OrganizationId != request.OrganizationId)
                throw new ConflictException("The parent department belongs to a different organization.");
        }

        var department = Department.Create(request.Name, request.OrganizationId, request.ParentDepartmentId);

        await _departments.AddAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return department.Id;
    }
}

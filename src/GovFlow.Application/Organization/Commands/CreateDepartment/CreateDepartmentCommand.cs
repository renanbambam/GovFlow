using MediatR;

namespace GovFlow.Application.Organization.Commands.CreateDepartment;

public sealed record CreateDepartmentCommand(
    string Name,
    Guid OrganizationId,
    Guid? ParentDepartmentId = null) : IRequest<Guid>;

using MediatR;

namespace GovFlow.Application.Organization.Commands.CreateDepartment;

/// <summary>Creates a department within an organization. Returns the new department id.</summary>
public sealed record CreateDepartmentCommand(
    string Name,
    Guid OrganizationId,
    Guid? ParentDepartmentId = null) : IRequest<Guid>;

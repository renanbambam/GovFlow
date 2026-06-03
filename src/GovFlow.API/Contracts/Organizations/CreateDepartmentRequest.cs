using GovFlow.Application.Organization.Commands.CreateDepartment;

namespace GovFlow.API.Contracts.Organizations;

/// <summary>Payload to create a department. The organization is taken from the route.</summary>
/// <param name="Name">Department name. Example: "Finance".</param>
/// <param name="ParentDepartmentId">Optional parent department for tree structures.</param>
public sealed record CreateDepartmentRequest(string Name, Guid? ParentDepartmentId = null)
{
    public CreateDepartmentCommand ToCommand(Guid organizationId) => new(Name, organizationId, ParentDepartmentId);
}

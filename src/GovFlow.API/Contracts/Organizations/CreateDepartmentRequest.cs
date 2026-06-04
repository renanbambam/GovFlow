using GovFlow.Application.Organization.Commands.CreateDepartment;

namespace GovFlow.API.Contracts.Organizations;

public sealed record CreateDepartmentRequest(string Name, Guid? ParentDepartmentId = null)
{
    public CreateDepartmentCommand ToCommand(Guid organizationId) => new(Name, organizationId, ParentDepartmentId);
}

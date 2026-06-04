using GovFlow.Application.Organization.Commands.CreateOrganization;

namespace GovFlow.API.Contracts.Organizations;

public sealed record CreateOrganizationRequest(string Name, string Slug)
{
    public CreateOrganizationCommand ToCommand() => new(Name, Slug);
}

using GovFlow.Application.Organization.Commands.CreateOrganization;

namespace GovFlow.API.Contracts.Organizations;

/// <summary>Payload to create a new organization (tenant).</summary>
/// <param name="Name">Human-readable organization name. Example: "City Hall".</param>
/// <param name="Slug">URL-safe unique identifier. Example: "city-hall".</param>
public sealed record CreateOrganizationRequest(string Name, string Slug)
{
    public CreateOrganizationCommand ToCommand() => new(Name, Slug);
}

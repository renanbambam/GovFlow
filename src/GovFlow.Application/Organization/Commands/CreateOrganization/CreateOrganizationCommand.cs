using MediatR;

namespace GovFlow.Application.Organization.Commands.CreateOrganization;

/// <summary>Creates a new tenant organization. Returns the new organization id.</summary>
public sealed record CreateOrganizationCommand(string Name, string Slug) : IRequest<Guid>;

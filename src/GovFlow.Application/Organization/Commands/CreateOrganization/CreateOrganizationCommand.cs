using MediatR;

namespace GovFlow.Application.Organization.Commands.CreateOrganization;

public sealed record CreateOrganizationCommand(string Name, string Slug) : IRequest<Guid>;

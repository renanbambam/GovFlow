using GovFlow.Application.Identity.Dtos;
using MediatR;

namespace GovFlow.Application.Identity.Commands.RegisterUser;

/// <summary>Registers a user in an organization and returns the initial token pair.</summary>
public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    Guid OrganizationId,
    string Role = "Analyst") : IRequest<AuthResponse>;

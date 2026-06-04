using GovFlow.Application.Identity.Dtos;
using MediatR;

namespace GovFlow.Application.Identity.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    Guid OrganizationId,
    string Role = "Analyst") : IRequest<AuthResponse>;

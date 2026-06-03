using GovFlow.Application.Identity.Dtos;
using MediatR;

namespace GovFlow.Application.Identity.Commands.LoginUser;

/// <summary>Authenticates a user by email + password and returns a token pair.</summary>
public sealed record LoginUserCommand(string Email, string Password) : IRequest<AuthResponse>;

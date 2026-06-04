using GovFlow.Application.Identity.Dtos;
using MediatR;

namespace GovFlow.Application.Identity.Commands.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<AuthResponse>;

using GovFlow.Application.Identity.Dtos;
using MediatR;

namespace GovFlow.Application.Identity.Commands.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;

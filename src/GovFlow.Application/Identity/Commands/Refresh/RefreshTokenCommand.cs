using GovFlow.Application.Identity.Dtos;
using MediatR;

namespace GovFlow.Application.Identity.Commands.Refresh;

/// <summary>Exchanges a valid refresh token for a new token pair.</summary>
public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;

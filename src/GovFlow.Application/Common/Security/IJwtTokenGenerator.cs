using GovFlow.Domain.Identity;

namespace GovFlow.Application.Common.Security;

/// <summary>Issues signed access tokens and refresh tokens for a user.</summary>
public interface IJwtTokenGenerator
{
    AuthTokens GenerateTokens(User user);
}

/// <summary>The token pair produced for an authenticated user.</summary>
public sealed record AuthTokens(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);

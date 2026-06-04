using GovFlow.Domain.Identity;

namespace GovFlow.Application.Common.Security;

public interface IJwtTokenGenerator
{
    AuthTokens GenerateTokens(User user);
}

public sealed record AuthTokens(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);

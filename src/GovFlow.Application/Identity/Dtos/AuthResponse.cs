namespace GovFlow.Application.Identity.Dtos;

/// <summary>Authentication result returned by register/login/refresh.</summary>
public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    IReadOnlyCollection<string> Roles);

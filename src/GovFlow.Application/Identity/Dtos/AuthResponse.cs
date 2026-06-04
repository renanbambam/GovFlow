namespace GovFlow.Application.Identity.Dtos;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    IReadOnlyCollection<string> Roles);

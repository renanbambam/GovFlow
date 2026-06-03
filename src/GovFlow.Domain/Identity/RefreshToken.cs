using GovFlow.Domain.Common;

namespace GovFlow.Domain.Identity;

/// <summary>
/// A persisted, revocable refresh token bound to a user. Simple by design: a single active
/// token chain where refreshing revokes the previous one.
/// </summary>
public sealed class RefreshToken : Entity
{
    public Guid UserId { get; private set; }

    public string Token { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;

    private RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Issue(Guid userId, string token, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token is required.", nameof(token));

        return new RefreshToken(userId, token, expiresAt);
    }

    public void Revoke()
    {
        RevokedAt ??= DateTime.UtcNow;
    }
}

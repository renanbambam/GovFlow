namespace GovFlow.Domain.Identity;

/// <summary>Persistence contract for <see cref="RefreshToken"/>.</summary>
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
}

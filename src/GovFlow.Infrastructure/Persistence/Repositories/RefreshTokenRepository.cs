using GovFlow.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.Repositories;

internal sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly GovFlowDbContext _context;

    public RefreshTokenRepository(GovFlowDbContext context) => _context = context;

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        => await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
}

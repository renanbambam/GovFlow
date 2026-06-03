using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Security;
using GovFlow.Application.Identity.Dtos;
using GovFlow.Domain.Common;
using GovFlow.Domain.Identity;
using MediatR;

namespace GovFlow.Application.Identity.Commands.Refresh;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existing = await _refreshTokens.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existing is null || !existing.IsActive)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        var user = await _users.GetByIdAsync(existing.UserId, cancellationToken)
            ?? throw new UnauthorizedException("Invalid or expired refresh token.");

        existing.Revoke();

        var tokens = _tokenGenerator.GenerateTokens(user);
        await _refreshTokens.AddAsync(
            RefreshToken.Issue(user.Id, tokens.RefreshToken, tokens.RefreshTokenExpiresAtUtc),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAtUtc,
            user.Id,
            user.Email,
            user.Roles);
    }
}

using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Security;
using GovFlow.Application.Identity.Dtos;
using GovFlow.Domain.Common;
using GovFlow.Domain.Identity;
using MediatR;

namespace GovFlow.Application.Identity.Commands.LoginUser;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserCommandHandler(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, cancellationToken);

        if (user is null || !user.IsActive || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

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

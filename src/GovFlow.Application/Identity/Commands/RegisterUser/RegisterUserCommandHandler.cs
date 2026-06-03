using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Security;
using GovFlow.Application.Identity.Dtos;
using GovFlow.Domain.Common;
using GovFlow.Domain.Identity;
using GovFlow.Domain.Organization;
using MediatR;

namespace GovFlow.Application.Identity.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IOrganizationRepository _organizations;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IOrganizationRepository organizations,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _organizations = organizations;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _users.EmailExistsAsync(email, cancellationToken))
            throw new ConflictException($"A user with email '{email}' already exists.");

        if (await _organizations.GetByIdAsync(request.OrganizationId, cancellationToken) is null)
            throw NotFoundException.For("Organization", request.OrganizationId);

        var user = User.Create(request.Name, email, _passwordHasher.Hash(request.Password), request.OrganizationId);
        user.AssignRole(request.Role);
        await _users.AddAsync(user, cancellationToken);

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

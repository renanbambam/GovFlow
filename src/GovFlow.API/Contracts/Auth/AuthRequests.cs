using GovFlow.Application.Identity.Commands.LoginUser;
using GovFlow.Application.Identity.Commands.Refresh;
using GovFlow.Application.Identity.Commands.RegisterUser;

namespace GovFlow.API.Contracts.Auth;

/// <summary>Registers a user. Role must be Admin, Manager or Analyst.</summary>
public sealed record RegisterRequest(
    string Name,
    string Email,
    string Password,
    Guid OrganizationId,
    string Role = "Analyst")
{
    public RegisterUserCommand ToCommand() => new(Name, Email, Password, OrganizationId, Role);
}

/// <summary>Authenticates with email and password.</summary>
public sealed record LoginRequest(string Email, string Password)
{
    public LoginUserCommand ToCommand() => new(Email, Password);
}

/// <summary>Exchanges a refresh token for a new token pair.</summary>
public sealed record RefreshRequest(string RefreshToken)
{
    public RefreshTokenCommand ToCommand() => new(RefreshToken);
}

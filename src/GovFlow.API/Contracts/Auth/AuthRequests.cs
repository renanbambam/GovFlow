using GovFlow.Application.Identity.Commands.LoginUser;
using GovFlow.Application.Identity.Commands.Refresh;
using GovFlow.Application.Identity.Commands.RegisterUser;

namespace GovFlow.API.Contracts.Auth;

public sealed record RegisterRequest(
    string Name,
    string Email,
    string Password,
    Guid OrganizationId,
    string Role = "Analyst")
{
    public RegisterUserCommand ToCommand() => new(Name, Email, Password, OrganizationId, Role);
}

public sealed record LoginRequest(string Email, string Password)
{
    public LoginUserCommand ToCommand() => new(Email, Password);
}

public sealed record RefreshRequest(string RefreshToken)
{
    public RefreshTokenCommand ToCommand() => new(RefreshToken);
}

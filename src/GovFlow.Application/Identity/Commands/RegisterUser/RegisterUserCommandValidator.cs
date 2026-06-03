using FluentValidation;

namespace GovFlow.Application.Identity.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private static readonly string[] AllowedRoles = { "Admin", "Manager", "Analyst" };

    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Role)
            .Must(role => AllowedRoles.Contains(role))
            .WithMessage("Role must be one of: Admin, Manager, Analyst.");
    }
}

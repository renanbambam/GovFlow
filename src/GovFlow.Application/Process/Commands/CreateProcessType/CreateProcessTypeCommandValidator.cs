using FluentValidation;

namespace GovFlow.Application.Process.Commands.CreateProcessType;

public sealed class CreateProcessTypeCommandValidator : AbstractValidator<CreateProcessTypeCommand>
{
    public CreateProcessTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.Steps)
            .NotEmpty()
            .WithMessage("A process type must define at least one workflow step.");

        RuleForEach(x => x.Steps).ChildRules(step =>
        {
            step.RuleFor(s => s.Name)
                .NotEmpty()
                .MaximumLength(200);

            step.RuleFor(s => s.SlaHours)
                .GreaterThan(0)
                .When(s => s.SlaHours.HasValue)
                .WithMessage("SLA hours must be positive when provided.");
        });
    }
}

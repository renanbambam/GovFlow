using FluentValidation;

namespace GovFlow.Application.Process.Commands.CompleteProcessStep;

public sealed class CompleteProcessStepCommandValidator : AbstractValidator<CompleteProcessStepCommand>
{
    public CompleteProcessStepCommandValidator()
    {
        RuleFor(x => x.ProcessInstanceId)
            .NotEmpty();

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => x.Notes is not null);
    }
}

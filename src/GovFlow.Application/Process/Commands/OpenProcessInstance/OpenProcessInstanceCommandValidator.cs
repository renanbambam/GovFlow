using FluentValidation;

namespace GovFlow.Application.Process.Commands.OpenProcessInstance;

public sealed class OpenProcessInstanceCommandValidator : AbstractValidator<OpenProcessInstanceCommand>
{
    public OpenProcessInstanceCommandValidator()
    {
        RuleFor(x => x.ProcessTypeId)
            .NotEmpty();

        RuleFor(x => x.RequesterId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}

using FluentValidation;

namespace GovFlow.Application.Process.Commands.AddProcessComment;

public sealed class AddProcessCommentCommandValidator : AbstractValidator<AddProcessCommentCommand>
{
    public AddProcessCommentCommandValidator()
    {
        RuleFor(x => x.ProcessInstanceId).NotEmpty();

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(4000);
    }
}

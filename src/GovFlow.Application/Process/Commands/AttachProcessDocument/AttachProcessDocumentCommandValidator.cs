using FluentValidation;

namespace GovFlow.Application.Process.Commands.AttachProcessDocument;

public sealed class AttachProcessDocumentCommandValidator : AbstractValidator<AttachProcessDocumentCommand>
{
    public const long MaxSizeBytes = 10 * 1024 * 1024;

    public AttachProcessDocumentCommandValidator()
    {
        RuleFor(x => x.ProcessInstanceId).NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(255)
            .Must(name => name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only PDF files are accepted.");

        RuleFor(x => x.ContentType)
            .Must(type => string.Equals(type, "application/pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only PDF files are accepted (content type must be application/pdf).");

        RuleFor(x => x.SizeBytes)
            .GreaterThan(0).WithMessage("The uploaded file is empty.")
            .LessThanOrEqualTo(MaxSizeBytes).WithMessage("The uploaded file exceeds the 10 MB limit.");
    }
}

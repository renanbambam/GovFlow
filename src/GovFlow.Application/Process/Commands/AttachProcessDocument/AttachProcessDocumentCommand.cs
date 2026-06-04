using MediatR;

namespace GovFlow.Application.Process.Commands.AttachProcessDocument;

public sealed record AttachProcessDocumentCommand(
    Guid ProcessInstanceId,
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream Content) : IRequest<Guid>;

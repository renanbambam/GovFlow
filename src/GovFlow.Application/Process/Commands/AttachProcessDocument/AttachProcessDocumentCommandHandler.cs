using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Interfaces;
using GovFlow.Domain.Common;
using GovFlow.Domain.Process;
using MediatR;

namespace GovFlow.Application.Process.Commands.AttachProcessDocument;

public sealed class AttachProcessDocumentCommandHandler : IRequestHandler<AttachProcessDocumentCommand, Guid>
{
    private readonly IProcessInstanceRepository _instances;
    private readonly IProcessDocumentRepository _documents;
    private readonly IFileStorageService _storage;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public AttachProcessDocumentCommandHandler(
        IProcessInstanceRepository instances,
        IProcessDocumentRepository documents,
        IFileStorageService storage,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _instances = instances;
        _documents = documents;
        _storage = storage;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AttachProcessDocumentCommand request, CancellationToken cancellationToken)
    {
        var uploaderId = _currentUser.UserId
            ?? throw new UnauthorizedException("An authenticated user is required to upload documents.");

        if (!await _instances.ExistsAsync(request.ProcessInstanceId, cancellationToken))
            throw NotFoundException.For("ProcessInstance", request.ProcessInstanceId);

        var stored = await _storage.SaveAsync(
            request.ProcessInstanceId.ToString(),
            request.FileName,
            request.Content,
            cancellationToken);

        var document = ProcessDocument.Create(
            request.ProcessInstanceId,
            uploaderId,
            request.FileName,
            request.ContentType,
            stored.SizeBytes,
            stored.StoragePath);

        await _documents.AddAsync(document, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return document.Id;
    }
}

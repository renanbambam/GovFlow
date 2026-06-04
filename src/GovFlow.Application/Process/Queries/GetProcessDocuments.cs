using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using MediatR;

namespace GovFlow.Application.Process.Queries;

public sealed record GetProcessDocumentsQuery(Guid ProcessId) : IRequest<IReadOnlyList<ProcessDocumentDto>>;

public sealed class GetProcessDocumentsQueryHandler
    : IRequestHandler<GetProcessDocumentsQuery, IReadOnlyList<ProcessDocumentDto>>
{
    private readonly IProcessDocumentReadRepository _documents;

    public GetProcessDocumentsQueryHandler(IProcessDocumentReadRepository documents) => _documents = documents;

    public async Task<IReadOnlyList<ProcessDocumentDto>> Handle(
        GetProcessDocumentsQuery request,
        CancellationToken cancellationToken)
        => await _documents.ListByProcessAsync(request.ProcessId, cancellationToken)
           ?? throw NotFoundException.For("ProcessInstance", request.ProcessId);
}

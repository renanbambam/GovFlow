using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using MediatR;

namespace GovFlow.Application.Process.Queries;

public sealed record GetProcessCommentsQuery(Guid ProcessId) : IRequest<IReadOnlyList<ProcessCommentDto>>;

public sealed class GetProcessCommentsQueryHandler
    : IRequestHandler<GetProcessCommentsQuery, IReadOnlyList<ProcessCommentDto>>
{
    private readonly IProcessCommentReadRepository _comments;

    public GetProcessCommentsQueryHandler(IProcessCommentReadRepository comments) => _comments = comments;

    public async Task<IReadOnlyList<ProcessCommentDto>> Handle(
        GetProcessCommentsQuery request,
        CancellationToken cancellationToken)
        => await _comments.ListByProcessAsync(request.ProcessId, cancellationToken)
           ?? throw NotFoundException.For("ProcessInstance", request.ProcessId);
}

using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using MediatR;

namespace GovFlow.Application.Process.Queries;

public sealed record GetProcessTimelineQuery(Guid ProcessId) : IRequest<IReadOnlyList<ProcessTimelineEntryDto>>;

public sealed class GetProcessTimelineQueryHandler
    : IRequestHandler<GetProcessTimelineQuery, IReadOnlyList<ProcessTimelineEntryDto>>
{
    private readonly IProcessReadRepository _processes;

    public GetProcessTimelineQueryHandler(IProcessReadRepository processes) => _processes = processes;

    public async Task<IReadOnlyList<ProcessTimelineEntryDto>> Handle(
        GetProcessTimelineQuery request,
        CancellationToken cancellationToken)
        => await _processes.GetTimelineAsync(request.ProcessId, cancellationToken)
           ?? throw NotFoundException.For("ProcessInstance", request.ProcessId);
}

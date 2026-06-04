using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using MediatR;

namespace GovFlow.Application.Process.Queries;

public sealed record GetProcessByIdQuery(Guid Id) : IRequest<ProcessInstanceDto>;

public sealed class GetProcessByIdQueryHandler : IRequestHandler<GetProcessByIdQuery, ProcessInstanceDto>
{
    private readonly IProcessReadRepository _processes;

    public GetProcessByIdQueryHandler(IProcessReadRepository processes) => _processes = processes;

    public async Task<ProcessInstanceDto> Handle(GetProcessByIdQuery request, CancellationToken cancellationToken)
        => await _processes.GetByIdAsync(request.Id, cancellationToken)
           ?? throw NotFoundException.For("ProcessInstance", request.Id);
}

using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Process.Dtos;
using MediatR;

namespace GovFlow.Application.Process.Queries;

public sealed record GetProcessTypeByIdQuery(Guid Id) : IRequest<ProcessTypeDto>;

public sealed class GetProcessTypeByIdQueryHandler : IRequestHandler<GetProcessTypeByIdQuery, ProcessTypeDto>
{
    private readonly IProcessTypeReadRepository _processTypes;

    public GetProcessTypeByIdQueryHandler(IProcessTypeReadRepository processTypes) => _processTypes = processTypes;

    public async Task<ProcessTypeDto> Handle(GetProcessTypeByIdQuery request, CancellationToken cancellationToken)
        => await _processTypes.GetByIdAsync(request.Id, cancellationToken)
           ?? throw NotFoundException.For("ProcessType", request.Id);
}

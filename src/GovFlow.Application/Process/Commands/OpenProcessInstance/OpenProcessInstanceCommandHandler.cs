using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Interfaces;
using GovFlow.Domain.Common;
using GovFlow.Domain.Process;
using MediatR;

namespace GovFlow.Application.Process.Commands.OpenProcessInstance;

public sealed class OpenProcessInstanceCommandHandler : IRequestHandler<OpenProcessInstanceCommand, Guid>
{
    private readonly IProcessTypeRepository _processTypes;
    private readonly IProcessInstanceRepository _instances;
    private readonly IProcessRealtimeNotifier _notifier;
    private readonly IUnitOfWork _unitOfWork;

    public OpenProcessInstanceCommandHandler(
        IProcessTypeRepository processTypes,
        IProcessInstanceRepository instances,
        IProcessRealtimeNotifier notifier,
        IUnitOfWork unitOfWork)
    {
        _processTypes = processTypes;
        _instances = instances;
        _notifier = notifier;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(OpenProcessInstanceCommand request, CancellationToken cancellationToken)
    {
        var processType = await _processTypes.GetByIdAsync(request.ProcessTypeId, cancellationToken)
            ?? throw NotFoundException.For("ProcessType", request.ProcessTypeId);

        var instance = ProcessInstance.Open(
            processType,
            request.RequesterId,
            request.Title,
            request.Description,
            request.Priority);

        await _instances.AddAsync(instance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notifier.ProcessStatusChangedAsync(instance.Id, instance.Status.ToString(), cancellationToken);

        return instance.Id;
    }
}

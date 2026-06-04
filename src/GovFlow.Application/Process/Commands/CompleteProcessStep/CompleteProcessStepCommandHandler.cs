using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Interfaces;
using GovFlow.Domain.Common;
using GovFlow.Domain.Process;
using MediatR;

namespace GovFlow.Application.Process.Commands.CompleteProcessStep;

public sealed class CompleteProcessStepCommandHandler : IRequestHandler<CompleteProcessStepCommand, Unit>
{
    private readonly IProcessInstanceRepository _instances;
    private readonly IProcessRealtimeNotifier _notifier;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteProcessStepCommandHandler(
        IProcessInstanceRepository instances,
        IProcessRealtimeNotifier notifier,
        IUnitOfWork unitOfWork)
    {
        _instances = instances;
        _notifier = notifier;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CompleteProcessStepCommand request, CancellationToken cancellationToken)
    {
        var instance = await _instances.GetByIdAsync(request.ProcessInstanceId, cancellationToken)
            ?? throw NotFoundException.For("ProcessInstance", request.ProcessInstanceId);

        instance.CompleteCurrentStep(request.Notes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notifier.ProcessStatusChangedAsync(instance.Id, instance.Status.ToString(), cancellationToken);

        return Unit.Value;
    }
}

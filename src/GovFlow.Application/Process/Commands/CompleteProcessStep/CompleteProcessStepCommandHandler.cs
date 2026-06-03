using GovFlow.Application.Common.Exceptions;
using GovFlow.Domain.Common;
using GovFlow.Domain.Process;
using MediatR;

namespace GovFlow.Application.Process.Commands.CompleteProcessStep;

public sealed class CompleteProcessStepCommandHandler : IRequestHandler<CompleteProcessStepCommand, Unit>
{
    private readonly IProcessInstanceRepository _instances;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteProcessStepCommandHandler(IProcessInstanceRepository instances, IUnitOfWork unitOfWork)
    {
        _instances = instances;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CompleteProcessStepCommand request, CancellationToken cancellationToken)
    {
        var instance = await _instances.GetByIdAsync(request.ProcessInstanceId, cancellationToken)
            ?? throw NotFoundException.For("ProcessInstance", request.ProcessInstanceId);

        instance.CompleteCurrentStep(request.Notes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

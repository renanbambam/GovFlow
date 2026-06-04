using GovFlow.Domain.Common;
using GovFlow.Domain.Process;
using MediatR;

namespace GovFlow.Application.Process.Commands.DetectStalledProcesses;

public sealed class DetectStalledProcessesCommandHandler : IRequestHandler<DetectStalledProcessesCommand, int>
{
    private readonly IProcessInstanceRepository _instances;
    private readonly IUnitOfWork _unitOfWork;

    public DetectStalledProcessesCommandHandler(IProcessInstanceRepository instances, IUnitOfWork unitOfWork)
    {
        _instances = instances;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(DetectStalledProcessesCommand request, CancellationToken cancellationToken)
    {
        var threshold = DateTime.UtcNow.AddDays(-Math.Abs(request.IdleDays));
        var stalledIds = await _instances.GetStalledProcessIdsAsync(threshold, cancellationToken);

        var breached = 0;
        foreach (var id in stalledIds)
        {
            var instance = await _instances.GetByIdAsync(id, cancellationToken);
            if (instance is not null && instance.RegisterSlaBreach(request.IdleDays))
                breached++;
        }

        if (breached > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return breached;
    }
}

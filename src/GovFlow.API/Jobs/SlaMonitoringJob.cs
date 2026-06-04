using GovFlow.Application.Process.Commands.DetectStalledProcesses;
using MediatR;

namespace GovFlow.API.Jobs;

public sealed class SlaMonitoringJob
{
    private readonly ISender _sender;
    private readonly ILogger<SlaMonitoringJob> _logger;

    public SlaMonitoringJob(ISender sender, ILogger<SlaMonitoringJob> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task RunAsync(int idleDays)
    {
        var breached = await _sender.Send(new DetectStalledProcessesCommand(idleDays));
        if (breached > 0)
            _logger.LogWarning("SLA monitor flagged {Count} stalled process(es) (idle > {IdleDays} day(s)).", breached, idleDays);
    }
}

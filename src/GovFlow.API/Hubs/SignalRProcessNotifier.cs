using GovFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GovFlow.API.Hubs;

internal sealed class SignalRProcessNotifier : IProcessRealtimeNotifier
{
    private readonly IHubContext<ProcessHub> _hub;

    public SignalRProcessNotifier(IHubContext<ProcessHub> hub) => _hub = hub;

    public Task ProcessStatusChangedAsync(Guid processId, string status, CancellationToken cancellationToken = default)
        => _hub.Clients
            .Group(ProcessHub.GroupFor(processId.ToString()))
            .SendAsync(
                ProcessHub.StatusChangedMethod,
                new ProcessStatusChange(processId, status, DateTime.UtcNow),
                cancellationToken);
}

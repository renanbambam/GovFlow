namespace GovFlow.Application.Common.Interfaces;

public sealed record ProcessStatusChange(Guid ProcessId, string Status, DateTime OccurredAt);

public interface IProcessRealtimeNotifier
{
    Task ProcessStatusChangedAsync(Guid processId, string status, CancellationToken cancellationToken = default);
}

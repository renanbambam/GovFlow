using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GovFlow.API.Hubs;

[Authorize]
public sealed class ProcessHub : Hub
{
    public const string StatusChangedMethod = "ProcessStatusChanged";

    public Task SubscribeToProcess(string processId)
        => Groups.AddToGroupAsync(Context.ConnectionId, GroupFor(processId));

    public Task UnsubscribeFromProcess(string processId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupFor(processId));

    internal static string GroupFor(string processId) => $"process:{processId}";
}

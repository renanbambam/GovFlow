using System.Net.Http.Json;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace GovFlow.Integration.Tests;

public class ProcessRealtimeTests : IClassFixture<GovFlowApiFactory>
{
    private readonly GovFlowApiFactory _factory;

    public ProcessRealtimeTests(GovFlowApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Completing_a_step_pushes_status_change_to_subscribers()
    {
        var token = _factory.GenerateAccessToken("Admin");
        var client = _factory.CreateAuthenticatedClient("Admin");

        var org = await (await client.PostAsJsonAsync("/api/v1/organizations",
            new { name = "Org RT", slug = $"org-rt-{Guid.NewGuid():N}" })).Content.ReadFromJsonAsync<CreatedDto>();
        var type = await (await client.PostAsJsonAsync("/api/v1/process-types", new
        {
            name = "T",
            description = "d",
            organizationId = org!.Id,
            steps = new[] { new { name = "s", description = "d" } }
        })).Content.ReadFromJsonAsync<CreatedDto>();
        var instance = await (await client.PostAsJsonAsync("/api/v1/processes", new
        {
            processTypeId = type!.Id,
            requesterId = Guid.NewGuid(),
            title = "p",
            description = "d",
            priority = "Normal"
        })).Content.ReadFromJsonAsync<CreatedDto>();

        await using var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(_factory.Server.BaseAddress, "hubs/processes"), HttpTransportType.LongPolling, options =>
            {
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .Build();

        var received = new TaskCompletionSource<ProcessStatusChangeDto>(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.On<ProcessStatusChangeDto>("ProcessStatusChanged", change => received.TrySetResult(change));

        await connection.StartAsync();
        await connection.InvokeAsync("SubscribeToProcess", instance!.Id.ToString());

        var complete = await client.PostAsJsonAsync(
            $"/api/v1/processes/{instance.Id}/complete-step", new { notes = "done" });
        complete.EnsureSuccessStatusCode();

        var completed = await Task.WhenAny(received.Task, Task.Delay(TimeSpan.FromSeconds(10)));
        Assert.True(completed == received.Task, "Did not receive a real-time status change in time.");
        var change = await received.Task;
        Assert.Equal(instance.Id, change.ProcessId);
        Assert.Equal("Resolved", change.Status);
    }

    private sealed record CreatedDto(Guid Id);

    private sealed record ProcessStatusChangeDto(Guid ProcessId, string Status, DateTime OccurredAt);
}

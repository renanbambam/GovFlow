using System.Net.Http.Json;
using GovFlow.Domain.Process;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GovFlow.Integration.Tests;

public class SlaMonitoringTests : IClassFixture<GovFlowApiFactory>
{
    private readonly GovFlowApiFactory _factory;

    public SlaMonitoringTests(GovFlowApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Stalled_query_detects_processes_without_recent_activity()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");

        var org = await (await client.PostAsJsonAsync("/api/v1/organizations",
            new { name = "Org SLA", slug = $"org-sla-{Guid.NewGuid():N}" })).Content.ReadFromJsonAsync<CreatedDto>();
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

        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IProcessInstanceRepository>();

        var stalledFuture = await repository.GetStalledProcessIdsAsync(DateTime.UtcNow.AddDays(1));
        Assert.Contains(instance!.Id, stalledFuture);

        var stalledPast = await repository.GetStalledProcessIdsAsync(DateTime.UtcNow.AddDays(-1));
        Assert.DoesNotContain(instance.Id, stalledPast);
    }

    private sealed record CreatedDto(Guid Id);
}

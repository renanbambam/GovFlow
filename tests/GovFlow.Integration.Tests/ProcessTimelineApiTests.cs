using System.Net;
using System.Net.Http.Json;
using GovFlow.Application.Process.Dtos;
using Xunit;

namespace GovFlow.Integration.Tests;

public class ProcessTimelineApiTests : IClassFixture<GovFlowApiFactory>
{
    private readonly GovFlowApiFactory _factory;

    public ProcessTimelineApiTests(GovFlowApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Timeline_reflects_full_process_lifecycle()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");

        var org = await (await client.PostAsJsonAsync("/api/v1/organizations",
            new { name = "Org TL", slug = "org-tl" })).Content.ReadFromJsonAsync<CreatedDto>();

        var type = await (await client.PostAsJsonAsync("/api/v1/process-types", new
        {
            name = "Simple",
            description = "one step",
            organizationId = org!.Id,
            steps = new[] { new { name = "Only step", description = "the only step" } }
        })).Content.ReadFromJsonAsync<CreatedDto>();

        var instance = await (await client.PostAsJsonAsync("/api/v1/processes", new
        {
            processTypeId = type!.Id,
            requesterId = Guid.NewGuid(),
            title = "Timeline process",
            description = "x",
            priority = "Normal"
        })).Content.ReadFromJsonAsync<CreatedDto>();

        var completeResponse = await client.PostAsJsonAsync($"/api/v1/processes/{instance!.Id}/complete-step", new { notes = "ok" });
        Assert.True(completeResponse.IsSuccessStatusCode, await completeResponse.Content.ReadAsStringAsync());

        var timeline = await client.GetFromJsonAsync<List<ProcessTimelineEntryDto>>(
            $"/api/v1/processes/{instance.Id}/timeline");

        Assert.NotNull(timeline);
        Assert.Equal(
            new[] { "ProcessOpened", "StepStarted", "StepCompleted", "ProcessResolved" },
            timeline!.Select(e => e.EventType).ToArray());
    }

    [Fact]
    public async Task Timeline_for_unknown_process_returns_404()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");

        var response = await client.GetAsync($"/api/v1/processes/{Guid.NewGuid()}/timeline");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record CreatedDto(Guid Id);
}

using System.Net;
using System.Net.Http.Json;
using GovFlow.Application.Process.Dtos;
using Xunit;

namespace GovFlow.Integration.Tests;

public class EndToEndFlowTests : IClassFixture<GovFlowApiFactory>
{
    private readonly GovFlowApiFactory _factory;

    public EndToEndFlowTests(GovFlowApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Create_organization_then_process_type_then_open_and_complete()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");

        var orgResponse = await client.PostAsJsonAsync("/api/v1/organizations",
            new { name = "City Hall", slug = "city-hall" });
        Assert.Equal(HttpStatusCode.Created, orgResponse.StatusCode);
        var organization = await orgResponse.Content.ReadFromJsonAsync<CreatedDto>();

        var typeResponse = await client.PostAsJsonAsync("/api/v1/process-types", new
        {
            name = "Vacation Request",
            description = "Employee vacation approval",
            organizationId = organization!.Id,
            steps = new[] { new { name = "Submit", description = "Employee submits" } }
        });
        Assert.Equal(HttpStatusCode.Created, typeResponse.StatusCode);
        var processType = await typeResponse.Content.ReadFromJsonAsync<CreatedDto>();

        var openResponse = await client.PostAsJsonAsync("/api/v1/processes", new
        {
            processTypeId = processType!.Id,
            requesterId = Guid.NewGuid(),
            title = "Vacation - July",
            description = "Two weeks",
            priority = "Normal"
        });
        Assert.Equal(HttpStatusCode.Created, openResponse.StatusCode);
        var instance = await openResponse.Content.ReadFromJsonAsync<CreatedDto>();

        var completeResponse = await client.PostAsJsonAsync(
            $"/api/v1/processes/{instance!.Id}/complete-step",
            new { notes = "done" });
        Assert.Equal(HttpStatusCode.NoContent, completeResponse.StatusCode);

        var detail = await client.GetFromJsonAsync<ProcessInstanceDto>($"/api/v1/processes/{instance.Id}");
        Assert.Equal("Resolved", detail!.Status);

        var list = await client.GetFromJsonAsync<List<ProcessSummaryDto>>("/api/v1/processes");
        Assert.Contains(list!, p => p.Id == instance.Id);

        var dashboard = await client.GetFromJsonAsync<DashboardSnapshot>("/api/v1/dashboard");
        Assert.Equal(1, dashboard!.TotalOrganizations);
        Assert.Equal(1, dashboard.TotalCompletedProcesses);
    }

    [Fact]
    public async Task Invalid_organization_payload_returns_400()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");

        var response = await client.PostAsJsonAsync("/api/v1/organizations",
            new { name = "", slug = "Invalid Slug!" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Opening_process_for_unknown_type_returns_404()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");

        var response = await client.PostAsJsonAsync("/api/v1/processes", new
        {
            processTypeId = Guid.NewGuid(),
            requesterId = Guid.NewGuid(),
            title = "x",
            description = "y",
            priority = "Normal"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record CreatedDto(Guid Id);

    private sealed record DashboardSnapshot(
        int TotalOrganizations,
        int TotalProcessTypes,
        int TotalOpenProcesses,
        int TotalCompletedProcesses,
        int TotalProcesses);
}

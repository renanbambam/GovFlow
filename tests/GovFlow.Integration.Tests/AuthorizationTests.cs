using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GovFlow.Integration.Tests;

public class AuthorizationTests : IClassFixture<GovFlowApiFactory>
{
    private readonly GovFlowApiFactory _factory;

    public AuthorizationTests(GovFlowApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Anonymous_request_to_protected_endpoint_is_401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/organizations");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Analyst_cannot_create_organization_403()
    {
        var client = _factory.CreateAuthenticatedClient("Analyst");

        var response = await client.PostAsJsonAsync("/api/v1/organizations",
            new { name = "Blocked", slug = "blocked-org" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Manager_is_authorized_for_process_types()
    {
        var client = _factory.CreateAuthenticatedClient("Manager");

        var response = await client.PostAsJsonAsync("/api/v1/process-types", new
        {
            name = "Type",
            description = "d",
            organizationId = Guid.NewGuid(),
            steps = new[] { new { name = "s", description = "d" } }
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

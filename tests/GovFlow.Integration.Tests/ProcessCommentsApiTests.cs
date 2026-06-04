using System.Net;
using System.Net.Http.Json;
using GovFlow.Application.Process.Dtos;
using Xunit;

namespace GovFlow.Integration.Tests;

public class ProcessCommentsApiTests : IClassFixture<GovFlowApiFactory>
{
    private readonly GovFlowApiFactory _factory;

    public ProcessCommentsApiTests(GovFlowApiFactory factory) => _factory = factory;

    private async Task<Guid> OpenProcessAsync(HttpClient client)
    {
        var org = await (await client.PostAsJsonAsync("/api/v1/organizations",
            new { name = "Org C", slug = $"org-c-{Guid.NewGuid():N}" })).Content.ReadFromJsonAsync<CreatedDto>();
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
        return instance!.Id;
    }

    [Fact]
    public async Task Can_add_and_list_comments()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");
        var processId = await OpenProcessAsync(client);

        var add = await client.PostAsJsonAsync($"/api/v1/processes/{processId}/comments",
            new { content = "First comment" });
        Assert.Equal(HttpStatusCode.Created, add.StatusCode);

        var comments = await client.GetFromJsonAsync<List<ProcessCommentDto>>(
            $"/api/v1/processes/{processId}/comments");

        var comment = Assert.Single(comments!);
        Assert.Equal("First comment", comment.Content);
        Assert.NotEqual(Guid.Empty, comment.AuthorId);
    }

    [Fact]
    public async Task Empty_comment_returns_400()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");
        var processId = await OpenProcessAsync(client);

        var response = await client.PostAsJsonAsync($"/api/v1/processes/{processId}/comments",
            new { content = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Comment_on_unknown_process_returns_404()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");

        var response = await client.PostAsJsonAsync($"/api/v1/processes/{Guid.NewGuid()}/comments",
            new { content = "hi" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record CreatedDto(Guid Id);
}

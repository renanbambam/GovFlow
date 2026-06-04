using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using GovFlow.Application.Process.Dtos;
using Xunit;

namespace GovFlow.Integration.Tests;

public class ProcessDocumentsApiTests : IClassFixture<GovFlowApiFactory>
{
    private readonly GovFlowApiFactory _factory;

    public ProcessDocumentsApiTests(GovFlowApiFactory factory) => _factory = factory;

    private async Task<Guid> OpenProcessAsync(HttpClient client)
    {
        var org = await (await client.PostAsJsonAsync("/api/v1/organizations",
            new { name = "Org D", slug = $"org-d-{Guid.NewGuid():N}" })).Content.ReadFromJsonAsync<CreatedDto>();
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

    private static MultipartFormDataContent PdfUpload(string fileName, string contentType = "application/pdf")
    {
        var bytes = Encoding.ASCII.GetBytes("%PDF-1.4\n%fake pdf for tests\n");
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return new MultipartFormDataContent { { fileContent, "file", fileName } };
    }

    [Fact]
    public async Task Can_upload_and_list_pdf_documents()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");
        var processId = await OpenProcessAsync(client);

        var upload = await client.PostAsync($"/api/v1/processes/{processId}/documents", PdfUpload("report.pdf"));
        Assert.Equal(HttpStatusCode.Created, upload.StatusCode);

        var documents = await client.GetFromJsonAsync<List<ProcessDocumentDto>>(
            $"/api/v1/processes/{processId}/documents");

        var document = Assert.Single(documents!);
        Assert.Equal("report.pdf", document.FileName);
        Assert.Equal("application/pdf", document.ContentType);
        Assert.True(document.SizeBytes > 0);
    }

    [Fact]
    public async Task Uploading_non_pdf_returns_400()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");
        var processId = await OpenProcessAsync(client);

        var upload = await client.PostAsync(
            $"/api/v1/processes/{processId}/documents", PdfUpload("photo.png", "image/png"));

        Assert.Equal(HttpStatusCode.BadRequest, upload.StatusCode);
    }

    [Fact]
    public async Task Upload_to_unknown_process_returns_404()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");

        var upload = await client.PostAsync(
            $"/api/v1/processes/{Guid.NewGuid()}/documents", PdfUpload("report.pdf"));

        Assert.Equal(HttpStatusCode.NotFound, upload.StatusCode);
    }

    private sealed record CreatedDto(Guid Id);
}

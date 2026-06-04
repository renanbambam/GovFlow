using System.Text;
using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Process.Commands.AttachProcessDocument;
using GovFlow.Application.Tests.Fakes;
using GovFlow.Domain.Process;
using Xunit;

namespace GovFlow.Application.Tests.Process;

public class AttachProcessDocumentCommandHandlerTests
{
    private static ProcessInstance BuildInstance()
    {
        var type = ProcessType.Create("p", "d", Guid.NewGuid());
        type.AddStep("s1", "first");
        return ProcessInstance.Open(type, Guid.NewGuid(), "t", "d");
    }

    private static AttachProcessDocumentCommand BuildCommand(Guid processId)
    {
        var content = new MemoryStream(Encoding.ASCII.GetBytes("%PDF-1.4 fake content"));
        return new AttachProcessDocumentCommand(processId, "report.pdf", "application/pdf", content.Length, content);
    }

    [Fact]
    public async Task Stores_file_and_persists_metadata()
    {
        var instances = new FakeProcessInstanceRepository();
        var documents = new FakeProcessDocumentRepository();
        var storage = new FakeFileStorageService();
        var instance = BuildInstance();
        instances.Items[instance.Id] = instance;
        var uploaderId = Guid.NewGuid();

        var handler = new AttachProcessDocumentCommandHandler(
            instances, documents, storage, new FakeCurrentUserService(uploaderId), new FakeUnitOfWork());

        var id = await handler.Handle(BuildCommand(instance.Id), CancellationToken.None);

        Assert.Single(storage.Saved);
        var document = Assert.Single(documents.Items);
        Assert.Equal(id, document.Id);
        Assert.Equal(uploaderId, document.UploadedByUserId);
        Assert.Equal("report.pdf", document.FileName);
        Assert.True(document.SizeBytes > 0);
    }

    [Fact]
    public async Task Throws_not_found_for_unknown_process()
    {
        var handler = new AttachProcessDocumentCommandHandler(
            new FakeProcessInstanceRepository(),
            new FakeProcessDocumentRepository(),
            new FakeFileStorageService(),
            new FakeCurrentUserService(Guid.NewGuid()),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(BuildCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public void Validator_rejects_non_pdf()
    {
        var validator = new AttachProcessDocumentCommandValidator();
        using var content = new MemoryStream(new byte[] { 1, 2, 3 });

        var result = validator.Validate(
            new AttachProcessDocumentCommand(Guid.NewGuid(), "image.png", "image/png", 3, content));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validator_rejects_oversized_file()
    {
        var validator = new AttachProcessDocumentCommandValidator();
        using var content = new MemoryStream();

        var result = validator.Validate(new AttachProcessDocumentCommand(
            Guid.NewGuid(), "big.pdf", "application/pdf", AttachProcessDocumentCommandValidator.MaxSizeBytes + 1, content));

        Assert.False(result.IsValid);
    }
}

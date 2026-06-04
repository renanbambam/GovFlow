using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Process.Commands.AddProcessComment;
using GovFlow.Application.Tests.Fakes;
using GovFlow.Domain.Process;
using Xunit;

namespace GovFlow.Application.Tests.Process;

public class AddProcessCommentCommandHandlerTests
{
    private static ProcessInstance BuildInstance()
    {
        var type = ProcessType.Create("p", "d", Guid.NewGuid());
        type.AddStep("s1", "first");
        return ProcessInstance.Open(type, Guid.NewGuid(), "t", "d");
    }

    [Fact]
    public async Task Adds_comment_for_existing_process()
    {
        var instances = new FakeProcessInstanceRepository();
        var comments = new FakeProcessCommentRepository();
        var instance = BuildInstance();
        instances.Items[instance.Id] = instance;
        var authorId = Guid.NewGuid();

        var handler = new AddProcessCommentCommandHandler(
            instances, comments, new FakeCurrentUserService(authorId), new FakeUnitOfWork());

        var id = await handler.Handle(
            new AddProcessCommentCommand(instance.Id, "Looks good"), CancellationToken.None);

        var stored = Assert.Single(comments.Items);
        Assert.Equal(id, stored.Id);
        Assert.Equal(authorId, stored.AuthorId);
        Assert.Equal("Looks good", stored.Content);
    }

    [Fact]
    public async Task Throws_not_found_for_unknown_process()
    {
        var handler = new AddProcessCommentCommandHandler(
            new FakeProcessInstanceRepository(),
            new FakeProcessCommentRepository(),
            new FakeCurrentUserService(Guid.NewGuid()),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new AddProcessCommentCommand(Guid.NewGuid(), "hi"), CancellationToken.None));
    }

    [Fact]
    public async Task Throws_unauthorized_when_no_current_user()
    {
        var instances = new FakeProcessInstanceRepository();
        var instance = BuildInstance();
        instances.Items[instance.Id] = instance;

        var handler = new AddProcessCommentCommandHandler(
            instances,
            new FakeProcessCommentRepository(),
            new FakeCurrentUserService(null),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => handler.Handle(new AddProcessCommentCommand(instance.Id, "hi"), CancellationToken.None));
    }
}

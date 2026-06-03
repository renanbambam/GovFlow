using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Organization.Commands.CreateOrganization;
using GovFlow.Application.Tests.Fakes;
using Xunit;
using OrganizationAggregate = GovFlow.Domain.Organization.Organization;

namespace GovFlow.Application.Tests.Organization;

public class CreateOrganizationCommandHandlerTests
{
    [Fact]
    public async Task Creates_organization_and_persists()
    {
        var repository = new FakeOrganizationRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateOrganizationCommandHandler(repository, unitOfWork);

        var id = await handler.Handle(new CreateOrganizationCommand("Acme", "acme"), CancellationToken.None);

        Assert.True(repository.Items.ContainsKey(id));
        Assert.Equal(1, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Duplicate_slug_throws_conflict()
    {
        var repository = new FakeOrganizationRepository();
        var existing = OrganizationAggregate.Create("Acme", "acme");
        repository.Items[existing.Id] = existing;
        var handler = new CreateOrganizationCommandHandler(repository, new FakeUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new CreateOrganizationCommand("Acme 2", "acme"), CancellationToken.None));
    }
}

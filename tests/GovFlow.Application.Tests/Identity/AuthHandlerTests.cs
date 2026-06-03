using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Identity.Commands.LoginUser;
using GovFlow.Application.Identity.Commands.RegisterUser;
using GovFlow.Application.Tests.Fakes;
using GovFlow.Domain.Identity;
using Xunit;
using OrganizationAggregate = GovFlow.Domain.Organization.Organization;

namespace GovFlow.Application.Tests.Identity;

public class AuthHandlerTests
{
    [Fact]
    public async Task Register_creates_user_and_returns_tokens()
    {
        var users = new FakeUserRepository();
        var orgs = new FakeOrganizationRepository();
        var org = OrganizationAggregate.Create("Acme", "acme");
        orgs.Items[org.Id] = org;
        var handler = new RegisterUserCommandHandler(
            users, new FakeRefreshTokenRepository(), orgs, new FakePasswordHasher(), new FakeJwtTokenGenerator(), new FakeUnitOfWork());

        var result = await handler.Handle(
            new RegisterUserCommand("Jane", "jane@govflow.local", "Passw0rd!", org.Id, "Manager"),
            CancellationToken.None);

        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.Contains("Manager", result.Roles);
        Assert.Single(users.Items);
    }

    [Fact]
    public async Task Register_with_duplicate_email_throws_conflict()
    {
        var users = new FakeUserRepository();
        var orgs = new FakeOrganizationRepository();
        var org = OrganizationAggregate.Create("Acme", "acme");
        orgs.Items[org.Id] = org;
        users.Items.Add(User.Create("Existing", "jane@govflow.local", "hashed:x", org.Id));
        var handler = new RegisterUserCommandHandler(
            users, new FakeRefreshTokenRepository(), orgs, new FakePasswordHasher(), new FakeJwtTokenGenerator(), new FakeUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new RegisterUserCommand("Jane", "jane@govflow.local", "Passw0rd!", org.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Register_for_unknown_organization_throws_not_found()
    {
        var handler = new RegisterUserCommandHandler(
            new FakeUserRepository(), new FakeRefreshTokenRepository(), new FakeOrganizationRepository(),
            new FakePasswordHasher(), new FakeJwtTokenGenerator(), new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(
            new RegisterUserCommand("Jane", "jane@govflow.local", "Passw0rd!", Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Login_with_valid_credentials_returns_tokens()
    {
        var hasher = new FakePasswordHasher();
        var users = new FakeUserRepository();
        users.Items.Add(User.Create("Jane", "jane@govflow.local", hasher.Hash("Passw0rd!"), Guid.NewGuid()));
        var handler = new LoginUserCommandHandler(
            users, new FakeRefreshTokenRepository(), hasher, new FakeJwtTokenGenerator(), new FakeUnitOfWork());

        var result = await handler.Handle(new LoginUserCommand("jane@govflow.local", "Passw0rd!"), CancellationToken.None);

        Assert.False(string.IsNullOrEmpty(result.AccessToken));
    }

    [Fact]
    public async Task Login_with_wrong_password_throws_unauthorized()
    {
        var hasher = new FakePasswordHasher();
        var users = new FakeUserRepository();
        users.Items.Add(User.Create("Jane", "jane@govflow.local", hasher.Hash("Passw0rd!"), Guid.NewGuid()));
        var handler = new LoginUserCommandHandler(
            users, new FakeRefreshTokenRepository(), hasher, new FakeJwtTokenGenerator(), new FakeUnitOfWork());

        await Assert.ThrowsAsync<UnauthorizedException>(() => handler.Handle(
            new LoginUserCommand("jane@govflow.local", "nope"), CancellationToken.None));
    }
}

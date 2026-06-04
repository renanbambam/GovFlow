using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GovFlow.Integration.Tests;

public class AuthFlowTests : IClassFixture<GovFlowApiFactory>
{
    private readonly GovFlowApiFactory _factory;

    public AuthFlowTests(GovFlowApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Register_then_login_and_use_token()
    {
        var admin = _factory.CreateAuthenticatedClient("Admin");
        var orgResponse = await admin.PostAsJsonAsync("/api/v1/organizations",
            new { name = "Auth Org", slug = "auth-org" });
        var org = await orgResponse.Content.ReadFromJsonAsync<CreatedDto>();

        var anon = _factory.CreateClient();

        var registerResponse = await anon.PostAsJsonAsync("/api/v1/auth/register", new
        {
            name = "Jane",
            email = "jane@govflow.local",
            password = "Passw0rd!",
            organizationId = org!.Id,
            role = "Manager"
        });
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
        var registered = await registerResponse.Content.ReadFromJsonAsync<AuthDto>();
        Assert.False(string.IsNullOrEmpty(registered!.AccessToken));
        Assert.Contains("Manager", registered.Roles);

        var loginResponse = await anon.PostAsJsonAsync("/api/v1/auth/login",
            new { email = "jane@govflow.local", password = "Passw0rd!" });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var badLogin = await anon.PostAsJsonAsync("/api/v1/auth/login",
            new { email = "jane@govflow.local", password = "wrong" });
        Assert.Equal(HttpStatusCode.Unauthorized, badLogin.StatusCode);
    }

    private sealed record CreatedDto(Guid Id);

    private sealed record AuthDto(string AccessToken, string RefreshToken, string[] Roles);
}

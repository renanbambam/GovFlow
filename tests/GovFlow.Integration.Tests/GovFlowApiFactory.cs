using System.Net.Http.Headers;
using GovFlow.Application.Common.Security;
using GovFlow.Domain.Identity;
using GovFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GovFlow.Integration.Tests;

/// <summary>
/// Boots the real API pipeline (controllers, MediatR, validation, exception handling) but
/// swaps the PostgreSQL context for an isolated in-memory database so the tests need no
/// running infrastructure.
/// </summary>
public sealed class GovFlowApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"govflow-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<GovFlowDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<GovFlowDbContext>();

            services.AddDbContext<GovFlowDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }

    /// <summary>Creates an HttpClient with a valid JWT carrying the given roles.</summary>
    public HttpClient CreateAuthenticatedClient(params string[] roles)
    {
        using var scope = Services.CreateScope();
        var tokenGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

        var user = User.Create("Test User", "test@govflow.local", "x", Guid.NewGuid());
        foreach (var role in roles)
            user.AssignRole(role);

        var tokens = tokenGenerator.GenerateTokens(user);

        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }
}

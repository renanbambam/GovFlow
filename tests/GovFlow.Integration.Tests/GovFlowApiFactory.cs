using System.Net.Http.Headers;
using GovFlow.Application.Common.Security;
using GovFlow.Domain.Identity;
using GovFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GovFlow.Integration.Tests;

public sealed class GovFlowApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"govflow-tests-{Guid.NewGuid()}";

    private readonly string _uploadsPath = Path.Combine(Path.GetTempPath(), $"govflow-tests-{Guid.NewGuid():N}");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FileStorage:BasePath"] = _uploadsPath
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<GovFlowDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<GovFlowDbContext>();

            services.AddDbContext<GovFlowDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }

    public HttpClient CreateAuthenticatedClient(params string[] roles)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateAccessToken(roles));
        return client;
    }

    public string GenerateAccessToken(params string[] roles)
    {
        using var scope = Services.CreateScope();
        var tokenGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

        var user = User.Create("Test User", "test@govflow.local", "x", Guid.NewGuid());
        foreach (var role in roles)
            user.AssignRole(role);

        return tokenGenerator.GenerateTokens(user).AccessToken;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && Directory.Exists(_uploadsPath))
            Directory.Delete(_uploadsPath, recursive: true);
    }
}

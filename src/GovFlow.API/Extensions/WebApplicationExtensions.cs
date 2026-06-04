using GovFlow.Infrastructure.Persistence;

namespace GovFlow.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return;

        var applyMigrations = app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");
        var seed = app.Configuration.GetValue<bool>("Database:Seed");
        if (!applyMigrations && !seed)
            return;

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GovFlowDbContext>();

        try
        {
            if (applyMigrations)
                await GovFlowDbInitializer.MigrateAsync(context);

            if (seed)
                await GovFlowDbInitializer.SeedAsync(context);
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Database initialization was skipped. Is PostgreSQL running (docker compose up -d)?");
        }
    }
}

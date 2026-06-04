using GovFlow.API.Jobs;
using Hangfire;
using Hangfire.InMemory;

namespace GovFlow.API.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddGovFlowBackgroundJobs(this IServiceCollection services)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseInMemoryStorage());

        services.AddHangfireServer();
        services.AddScoped<SlaMonitoringJob>();

        return services;
    }

    public static WebApplication UseGovFlowBackgroundJobs(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseHangfireDashboard("/hangfire");

        var idleDays = app.Configuration.GetValue<int?>("Sla:IdleDays") ?? 3;
        var cron = app.Configuration.GetValue<string>("Sla:Cron") ?? Cron.Hourly();

        var recurring = app.Services.GetRequiredService<IRecurringJobManager>();
        recurring.AddOrUpdate<SlaMonitoringJob>(
            "sla-monitoring",
            job => job.RunAsync(idleDays),
            cron);

        return app;
    }
}

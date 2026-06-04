using GovFlow.Domain.Identity;
using GovFlow.Domain.Process;
using Microsoft.EntityFrameworkCore;
using OrganizationAggregate = GovFlow.Domain.Organization.Organization;

namespace GovFlow.Infrastructure.Persistence;

public static class GovFlowDbInitializer
{
    public static async Task MigrateAsync(GovFlowDbContext context, CancellationToken cancellationToken = default)
    {
        if (context.Database.IsRelational())
            await context.Database.MigrateAsync(cancellationToken);
    }

    public static async Task SeedAsync(GovFlowDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Organizations.AnyAsync(cancellationToken))
            return;

        var organization = OrganizationAggregate.Create("Demo Organization", "demo");
        await context.Organizations.AddAsync(organization, cancellationToken);

        var processType = ProcessType.Create("Onboarding", "Sample onboarding workflow", organization.Id);
        processType.AddStep("Submit request", "The requester submits the request.");
        processType.AddStep("Analyst review", "An analyst reviews and approves the request.");
        await context.ProcessTypes.AddAsync(processType, cancellationToken);

        var admin = User.Create(
            "Demo Admin",
            "admin@govflow.local",
            BCrypt.Net.BCrypt.HashPassword("Admin#12345"),
            organization.Id);
        admin.AssignRole("Admin");
        await context.Users.AddAsync(admin, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}

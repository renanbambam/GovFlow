using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GovFlow.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by the EF Core tools (<c>dotnet ef migrations</c>) so they can
/// build the model without booting the API host. The connection string is only used when a
/// command actually touches the database; <c>migrations add</c> does not.
/// </summary>
public sealed class GovFlowDbContextFactory : IDesignTimeDbContextFactory<GovFlowDbContext>
{
    public GovFlowDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=govflow;Username=govflow;Password=govflow";

        var options = new DbContextOptionsBuilder<GovFlowDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new GovFlowDbContext(options);
    }
}

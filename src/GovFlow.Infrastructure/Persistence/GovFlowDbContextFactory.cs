using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GovFlow.Infrastructure.Persistence;

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

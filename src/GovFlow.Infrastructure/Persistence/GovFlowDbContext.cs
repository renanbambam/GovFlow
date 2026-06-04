using System.Reflection;
using GovFlow.Domain.Common;
using GovFlow.Domain.Identity;
using GovFlow.Domain.Organization;
using GovFlow.Domain.Process;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence;

public sealed class GovFlowDbContext : DbContext, IUnitOfWork
{
    public GovFlowDbContext(DbContextOptions<GovFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ProcessType> ProcessTypes => Set<ProcessType>();
    public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
    public DbSet<ProcessInstance> ProcessInstances => Set<ProcessInstance>();
    public DbSet<ProcessInstanceStep> ProcessInstanceSteps => Set<ProcessInstanceStep>();
    public DbSet<ProcessTimelineEntry> ProcessTimelineEntries => Set<ProcessTimelineEntry>();
    public DbSet<ProcessComment> ProcessComments => Set<ProcessComment>();
    public DbSet<ProcessDocument> ProcessDocuments => Set<ProcessDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}

using GovFlow.Domain.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovFlow.Infrastructure.Persistence.Configurations;

internal sealed class ProcessInstanceStepConfiguration : IEntityTypeConfiguration<ProcessInstanceStep>
{
    public void Configure(EntityTypeBuilder<ProcessInstanceStep> builder)
    {
        builder.ToTable("process_instance_steps");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProcessInstanceId).IsRequired();
        builder.Property(x => x.WorkflowStepId).IsRequired();
        builder.Property(x => x.Sequence).IsRequired();
        builder.Property(x => x.AssignedUserId);
        builder.Property(x => x.AssignedDepartmentId);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.StartedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.CompletedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.Notes).HasMaxLength(2000);

        builder.HasIndex(x => new { x.ProcessInstanceId, x.Sequence });
    }
}

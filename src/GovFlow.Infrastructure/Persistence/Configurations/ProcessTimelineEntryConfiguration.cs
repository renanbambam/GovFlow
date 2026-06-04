using GovFlow.Domain.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovFlow.Infrastructure.Persistence.Configurations;

internal sealed class ProcessTimelineEntryConfiguration : IEntityTypeConfiguration<ProcessTimelineEntry>
{
    public void Configure(EntityTypeBuilder<ProcessTimelineEntry> builder)
    {
        builder.ToTable("process_timeline_entries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ProcessInstanceId).IsRequired();
        builder.Property(x => x.Sequence).IsRequired();

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.StepId);
        builder.Property(x => x.OccurredAt).IsRequired().HasColumnType("timestamp with time zone");

        builder.HasIndex(x => new { x.ProcessInstanceId, x.Sequence });
    }
}

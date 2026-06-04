using GovFlow.Domain.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovFlow.Infrastructure.Persistence.Configurations;

internal sealed class ProcessCommentConfiguration : IEntityTypeConfiguration<ProcessComment>
{
    public void Configure(EntityTypeBuilder<ProcessComment> builder)
    {
        builder.ToTable("process_comments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ProcessInstanceId).IsRequired();
        builder.Property(x => x.AuthorId).IsRequired();
        builder.Property(x => x.Content).IsRequired().HasMaxLength(4000);
        builder.Property(x => x.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.ProcessInstanceId);
    }
}

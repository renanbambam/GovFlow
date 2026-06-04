using GovFlow.Domain.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovFlow.Infrastructure.Persistence.Configurations;

internal sealed class ProcessDocumentConfiguration : IEntityTypeConfiguration<ProcessDocument>
{
    public void Configure(EntityTypeBuilder<ProcessDocument> builder)
    {
        builder.ToTable("process_documents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ProcessInstanceId).IsRequired();
        builder.Property(x => x.UploadedByUserId).IsRequired();
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SizeBytes).IsRequired();
        builder.Property(x => x.StoragePath).IsRequired().HasMaxLength(500);
        builder.Property(x => x.UploadedAt).IsRequired().HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.ProcessInstanceId);
    }
}

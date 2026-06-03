using GovFlow.Domain.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovFlow.Infrastructure.Persistence.Configurations;

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");
        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.ParentDepartmentId);
        builder.Property(x => x.ManagerUserId);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.OrganizationId);
        builder.HasIndex(x => x.ParentDepartmentId);
    }
}

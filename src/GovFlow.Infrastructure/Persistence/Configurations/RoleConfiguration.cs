using System.Text.Json;
using GovFlow.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovFlow.Infrastructure.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");

        // Permission codes are a case-insensitive set in a private backing field, persisted as
        // JSON. A value comparer lets EF detect set changes correctly.
        builder.Ignore(x => x.PermissionCodes);
        builder.Property<HashSet<string>>("_permissionCodes")
            .HasColumnName("permission_codes")
            .HasConversion(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<HashSet<string>>(json, (JsonSerializerOptions?)null)
                        ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                new ValueComparer<HashSet<string>>(
                    (left, right) => left!.SetEquals(right!),
                    value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                    value => new HashSet<string>(value, StringComparer.OrdinalIgnoreCase)));

        builder.HasIndex(x => x.OrganizationId);
    }
}

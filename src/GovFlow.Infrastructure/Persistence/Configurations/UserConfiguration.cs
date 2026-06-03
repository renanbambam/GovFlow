using System.Text.Json;
using GovFlow.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovFlow.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(320);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);
        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.DepartmentId);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");

        // Role assignments are held by id in a private backing field, persisted as JSON so the
        // mapping is portable across providers.
        builder.Ignore(x => x.RoleIds);
        builder.Property<List<Guid>>("_roleIds")
            .HasColumnName("role_ids")
            .HasConversion(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<List<Guid>>(json, (JsonSerializerOptions?)null) ?? new List<Guid>(),
                new ValueComparer<List<Guid>>(
                    (left, right) => left!.SequenceEqual(right!),
                    value => value.Aggregate(0, (hash, id) => HashCode.Combine(hash, id.GetHashCode())),
                    value => value.ToList()));

        // Role names (Admin/Manager/Analyst) used for authorization, persisted as JSON.
        builder.Ignore(x => x.Roles);
        builder.Property<HashSet<string>>("_roles")
            .HasColumnName("roles")
            .HasConversion(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<HashSet<string>>(json, (JsonSerializerOptions?)null)
                        ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                new ValueComparer<HashSet<string>>(
                    (left, right) => left!.SetEquals(right!),
                    value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                    value => new HashSet<string>(value, StringComparer.OrdinalIgnoreCase)));

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.OrganizationId);
    }
}

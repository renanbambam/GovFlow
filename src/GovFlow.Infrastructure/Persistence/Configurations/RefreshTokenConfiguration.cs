using GovFlow.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovFlow.Infrastructure.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Token).IsRequired().HasMaxLength(200);
        builder.Property(x => x.ExpiresAt).IsRequired().HasColumnType("timestamp with time zone");
        builder.Property(x => x.RevokedAt).HasColumnType("timestamp with time zone");
        builder.Ignore(x => x.IsActive);

        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasIndex(x => x.UserId);
    }
}

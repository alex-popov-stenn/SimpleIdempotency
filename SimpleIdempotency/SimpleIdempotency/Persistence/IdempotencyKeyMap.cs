using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdempotency.Domain;

namespace SimpleIdempotency.Persistence;

internal sealed class IdempotencyKeyMap : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
    {
        builder.HasKey(x => x.Key);
        builder.Property(x => x.Key).HasMaxLength(128).IsRequired();
        builder.HasIndex(o => new { o.Namespace, o.Key })
            .HasDatabaseName("IX_IdempotencyKey_Uniqueness")
            .IsUnique();
    }
}
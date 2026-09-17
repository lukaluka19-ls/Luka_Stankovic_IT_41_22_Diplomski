using Igraonica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Igraonica.Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(100);
        builder.Property(m => m.Recipient).HasMaxLength(256).IsRequired();
        builder.Property(m => m.Subject).HasMaxLength(300).IsRequired();
        builder.Property(m => m.Body).HasColumnType("text").IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();

        builder.HasIndex(m => m.SentAt).HasDatabaseName("IX_OutboxMessages_SentAt");
    }
}

using Igraonica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Igraonica.Infrastructure.Persistence.Configurations;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("TimeSlots");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.StartTime).HasColumnType("time").IsRequired();
        builder.Property(t => t.EndTime).HasColumnType("time").IsRequired();
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.IsActive).HasDefaultValue(true);
    }
}

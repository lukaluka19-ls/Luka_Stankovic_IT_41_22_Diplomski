using Igraonica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Igraonica.Infrastructure.Persistence.Configurations;

public class ReservationCancellationConfiguration : IEntityTypeConfiguration<ReservationCancellation>
{
    public void Configure(EntityTypeBuilder<ReservationCancellation> builder)
    {
        builder.ToTable("ReservationCancellations");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Reason).HasMaxLength(500).IsRequired();
        builder.Property(c => c.CancelledAt).IsRequired();

        builder.HasOne<Reservation>().WithMany().HasForeignKey(c => c.ReservationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.CancelledByUserId).OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_ReservationCancellations_ReasonLength", "length(\"Reason\") >= 10"));
    }
}

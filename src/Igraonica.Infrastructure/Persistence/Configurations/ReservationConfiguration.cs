using Igraonica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Igraonica.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReservationNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(r => r.ReservationNumber).IsUnique();

        builder.Property(r => r.ChildName).HasMaxLength(100).IsRequired();
        builder.Property(r => r.EventDate).HasColumnType("date").IsRequired();
        builder.Property(r => r.TotalPrice).HasPrecision(10, 2);
        builder.Property(r => r.Status).HasConversion<short>();
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.ConfirmationDeadline).IsRequired();

        builder.HasOne<User>().WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<TimeSlot>().WithMany().HasForeignKey(r => r.TimeSlotId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Package>().WithMany().HasForeignKey(r => r.PackageId).OnDelete(DeleteBehavior.Restrict);

        // Delimicni jedinstveni indeks: sprecava da dva istovremena zahteva
        // zauzmu isti termin. Primenjuje se samo na aktivne rezervacije,
        // pa otkazane i istekle ostaju u tabeli radi istorije, ali ne
        // blokiraju termin.
        builder.HasIndex(r => new { r.EventDate, r.TimeSlotId })
            .IsUnique()
            .HasFilter("\"Status\" IN (0, 1)")
            .HasDatabaseName("IX_Reservations_ActiveSlot");

        builder.HasIndex(r => r.EventDate).HasDatabaseName("IX_Reservations_EventDate");
        builder.HasIndex(r => r.UserId).HasDatabaseName("IX_Reservations_UserId");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Reservations_ChildAge", "\"ChildAge\" BETWEEN 1 AND 14");
            t.HasCheckConstraint("CK_Reservations_Children", "\"NumberOfChildren\" BETWEEN 1 AND 40");
            t.HasCheckConstraint("CK_Reservations_TotalPrice", "\"TotalPrice\" >= 0");
        });
    }
}

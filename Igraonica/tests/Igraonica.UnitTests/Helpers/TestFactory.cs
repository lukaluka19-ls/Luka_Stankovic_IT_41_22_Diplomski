using Igraonica.Domain.Entities;
using Igraonica.Domain.Enums;

namespace Igraonica.UnitTests.Helpers;

internal static class TestFactory
{
    public static Package CreatePackage(decimal basePrice = 9000m, decimal pricePerChild = 900m) =>
        Package.Create(PackageType.Basic, "Osnovni paket", "Opis paketa", basePrice, pricePerChild);

    public static Reservation CreateReservation(
        DateTimeOffset now, DateOnly? eventDate = null, int confirmationDeadlineDays = 5, Package? package = null)
    {
        package ??= CreatePackage();
        var date = eventDate ?? DateOnly.FromDateTime(now.Date).AddDays(30);

        return Reservation.Create(
            userId: Guid.NewGuid(),
            childName: "Marko",
            childAge: 6,
            eventDate: date,
            timeSlotId: Guid.NewGuid(),
            numberOfChildren: 10,
            package: package,
            now: now,
            confirmationDeadlineDays: confirmationDeadlineDays);
    }
}

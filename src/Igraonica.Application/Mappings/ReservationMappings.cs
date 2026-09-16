using Igraonica.Application.DTOs;
using Igraonica.Domain.Entities;

namespace Igraonica.Application.Mappings;

public static class ReservationMappings
{
    public static ReservationDto ToDto(this Reservation reservation, Package package,
        TimeSlot timeSlot, DateTimeOffset now)
    {
        int? daysUntilDeadline = reservation.Status == Domain.Enums.ReservationStatus.PendingConfirmation
            ? Math.Max(0, (int)Math.Ceiling((reservation.ConfirmationDeadline - now).TotalDays))
            : null;

        return new ReservationDto(
            reservation.Id,
            reservation.ReservationNumber,
            reservation.UserId,
            reservation.ChildName,
            reservation.ChildAge,
            reservation.EventDate,
            reservation.TimeSlotId,
            timeSlot.Name,
            reservation.NumberOfChildren,
            reservation.PackageId,
            package.Name,
            reservation.Status.ToString(),
            reservation.TotalPrice,
            reservation.CreatedAt,
            reservation.ConfirmationDeadline,
            reservation.ConfirmedAt,
            reservation.CancelledAt,
            daysUntilDeadline);
    }
}

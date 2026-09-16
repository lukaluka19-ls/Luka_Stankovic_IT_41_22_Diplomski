namespace Igraonica.Application.DTOs;

public sealed record AvailabilityDayDto(
    DateOnly Date, bool IsWeekend, int Capacity, int Booked, int Remaining);

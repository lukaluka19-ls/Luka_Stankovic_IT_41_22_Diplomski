namespace Igraonica.Application.DTOs;

public sealed record TimeSlotDto(
    Guid Id, TimeOnly StartTime, TimeOnly EndTime, string Name, bool IsActive);

public sealed record CreateTimeSlotRequest(TimeOnly StartTime, TimeOnly EndTime, string Name);

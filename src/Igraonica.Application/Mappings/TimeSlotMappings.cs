using Igraonica.Application.DTOs;
using Igraonica.Domain.Entities;

namespace Igraonica.Application.Mappings;

public static class TimeSlotMappings
{
    public static TimeSlotDto ToDto(this TimeSlot timeSlot) => new(
        timeSlot.Id, timeSlot.StartTime, timeSlot.EndTime, timeSlot.Name, timeSlot.IsActive);
}

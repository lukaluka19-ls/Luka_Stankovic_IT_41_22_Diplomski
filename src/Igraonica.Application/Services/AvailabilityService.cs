using Igraonica.Application.DTOs;
using Igraonica.Application.Options;
using Igraonica.Domain.Repositories;
using Igraonica.Domain.Rules;
using Microsoft.Extensions.Options;

namespace Igraonica.Application.Services;

public class AvailabilityService(
    IReservationRepository _reservationRepository,
    IOptions<BusinessRulesOptions> _rulesOptions) : IAvailabilityService
{
    public async Task<IReadOnlyList<AvailabilityDayDto>> GetAvailabilityAsync(
        DateOnly from, DateOnly to, CancellationToken ct)
    {
        if (to < from)
            (from, to) = (to, from);

        var rules = _rulesOptions.Value;
        var counts = await _reservationRepository.GetActiveCountsInRangeAsync(from, to, ct);

        var days = new List<AvailabilityDayDto>();
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            var isWeekend = CapacityRules.IsWeekend(date);
            var capacity = CapacityRules.GetDailyCapacity(date, rules.WeekdayCapacity, rules.WeekendCapacity);
            var booked = counts.TryGetValue(date, out var c) ? c : 0;

            days.Add(new AvailabilityDayDto(date, isWeekend, capacity, booked, Math.Max(0, capacity - booked)));
        }

        return days;
    }
}

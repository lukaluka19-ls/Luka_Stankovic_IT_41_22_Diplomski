using Igraonica.Domain.Exceptions;

namespace Igraonica.Domain.Rules;

/// <summary>
/// Pravilo kapaciteta: radnim danima najvise jedna proslava dnevno,
/// vikendom najvise tri. Izdvojeno u zaseban tip jer predstavlja
/// poslovno pravilo koje se u buducnosti moze promeniti
/// (na primer uvodjenjem drzavnih praznika kao dana sa vikend kapacitetom).
/// </summary>
public static class CapacityRules
{
    public static bool IsWeekend(DateOnly date) =>
        date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    public static int GetDailyCapacity(DateOnly date, int weekdayCapacity, int weekendCapacity) =>
        IsWeekend(date) ? weekendCapacity : weekdayCapacity;

    public static void EnsureCapacityAvailable(DateOnly date, int existingCount,
        int weekdayCapacity, int weekendCapacity)
    {
        var capacity = GetDailyCapacity(date, weekdayCapacity, weekendCapacity);

        if (existingCount >= capacity)
            throw new CapacityExceededException(
                $"Za datum {date:dd.MM.yyyy.} vec postoji maksimalan broj " +
                $"rezervacija ({capacity}). Molimo izaberite drugi datum.");
    }
}

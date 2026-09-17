using Igraonica.Application.Abstractions;

namespace Igraonica.UnitTests.Helpers;

/// <summary>
/// Omogucava da se u testovima simulira protok vremena bez oslanjanja na
/// stvarni sistemski sat.
/// </summary>
public class FakeDateTimeProvider(DateTimeOffset initial) : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; private set; } = initial;

    public void Advance(TimeSpan span) => UtcNow = UtcNow.Add(span);
}

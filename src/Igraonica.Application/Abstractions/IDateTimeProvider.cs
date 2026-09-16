namespace Igraonica.Application.Abstractions;

/// <summary>
/// Apstrakcija tekuceg vremena. Omogucava da se u testovima simulira
/// protok vremena bez oslanjanja na stvarni sistemski sat.
/// </summary>
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}

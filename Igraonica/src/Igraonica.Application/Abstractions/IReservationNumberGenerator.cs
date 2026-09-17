namespace Igraonica.Application.Abstractions;

public interface IReservationNumberGenerator
{
    Task<string> NextAsync(CancellationToken ct);
}

using Igraonica.Application.Abstractions;
using Igraonica.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Igraonica.Infrastructure.Services;

/// <summary>
/// Koristi bazu sekvencu (reservation_number_seq) koja garantuje jedinstvenost
/// nezavisno od nivoa izolacije transakcije koja kreira rezervaciju.
/// </summary>
public class ReservationNumberGenerator(ApplicationDbContext _db) : IReservationNumberGenerator
{
    public async Task<string> NextAsync(CancellationToken ct)
    {
        var connection = _db.Database.GetDbConnection();
        var wasClosed = connection.State != System.Data.ConnectionState.Open;
        if (wasClosed)
            await connection.OpenAsync(ct);

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT nextval('reservation_number_seq')";
            var result = await command.ExecuteScalarAsync(ct);
            var sequenceValue = Convert.ToInt64(result);

            return $"REZ-{DateTime.UtcNow.Year}-{sequenceValue:D5}";
        }
        finally
        {
            if (wasClosed)
                await connection.CloseAsync();
        }
    }
}

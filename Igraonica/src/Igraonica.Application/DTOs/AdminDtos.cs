namespace Igraonica.Application.DTOs;

public sealed record AuditLogDto(
    Guid Id, Guid UserId, string Action, string EntityName, string Details, DateTimeOffset Timestamp);

public sealed record PackagePopularityDto(string PackageName, int Count);

public sealed record StatisticsDto(
    int TotalReservations,
    int PendingConfirmation,
    int Confirmed,
    int Completed,
    int CancelledByClient,
    int CancelledByAdmin,
    int AutoExpired,
    decimal TotalRevenue,
    IReadOnlyList<PackagePopularityDto> PopularPackages);

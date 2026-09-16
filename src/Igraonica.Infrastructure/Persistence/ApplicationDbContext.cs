using Igraonica.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Igraonica.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<ReservationCancellation> ReservationCancellations => Set<ReservationCancellation>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

using Igraonica.Application.Abstractions;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Igraonica.Infrastructure.Persistence.Seed;

/// <summary>
/// Popunjava bazu pocetnim podacima: nalog administratora, ponudu paketa i
/// raspolozive termine igraonice JoyLand (Karadjordjeva 18, Smederevo).
/// Pokrece se samo u razvojnom okruzenju, nakon primene migracija.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var hasher = services.GetRequiredService<IPasswordHasher>();
        var clock = services.GetRequiredService<IDateTimeProvider>();

        await SeedSequenceAsync(db);

        if (!await db.Users.AnyAsync(u => u.Role == UserRole.Administrator))
        {
            var admin = User.CreateAdministrator(
                "admin@joyland.local", hasher.Hash("Administrator123!"),
                "Administrator", "JoyLand", "063645886", clock.UtcNow);
            await db.Users.AddAsync(admin);
        }

        if (!await db.Users.AnyAsync(u => u.Email == "roditelj@example.com"))
        {
            var demoParent = User.Register(
                "roditelj@example.com", hasher.Hash("Roditelj123!"),
                "Ana", "Jovanovic", "0641234567", "seed-verified", clock.UtcNow);
            demoParent.VerifyEmail("seed-verified");
            await db.Users.AddAsync(demoParent);
        }

        if (!await db.Packages.AnyAsync())
        {
            await db.Packages.AddRangeAsync(
                Package.Create(PackageType.Basic, "Osnovni paket",
                    "Dva sata druzenja u igraonici uz kafu i pice za odrasle, hranu i pice " +
                    "za svu decu, kompletnu uslugu osoblja i ukras za sto rodjendanskog deteta.",
                    basePrice: 12000m, pricePerChild: 940m), // ~8 EUR po gostu
                Package.Create(PackageType.Standard, "Standardni paket",
                    "Sve iz osnovnog paketa uz animatora sa radionicom, torta po izboru " +
                    "i fotografisanje dece uz uspomenu za poneti.",
                    basePrice: 13000m, pricePerChild: 1100m),
                Package.Create(PackageType.Premium, "Premium paket",
                    "Sve iz standardnog paketa uz oslikavanje lica, tematsku dekoraciju " +
                    "sale i poklon paket za svako dete.",
                    basePrice: 18000m, pricePerChild: 1400m));
        }

        if (!await db.TimeSlots.AnyAsync())
        {
            await db.TimeSlots.AddRangeAsync(
                TimeSlot.Create(new TimeOnly(10, 0), new TimeOnly(12, 0), "Prepodnevni termin"),
                TimeSlot.Create(new TimeOnly(13, 0), new TimeOnly(15, 0), "Ranopopodnevni termin"),
                TimeSlot.Create(new TimeOnly(16, 30), new TimeOnly(18, 30), "Popodnevni termin"));
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedSequenceAsync(ApplicationDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync(
            "CREATE SEQUENCE IF NOT EXISTS reservation_number_seq START WITH 1 INCREMENT BY 1;");
    }
}

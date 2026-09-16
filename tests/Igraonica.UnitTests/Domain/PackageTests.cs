using FluentAssertions;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Enums;
using Xunit;

namespace Igraonica.UnitTests.Domain;

public class PackageTests
{
    // T-14: obracun cene za sva tri tipa paketa - tacan iznos (PP-08)
    [Theory]
    [InlineData(PackageType.Basic, 9000, 900, 10, 18000)]
    [InlineData(PackageType.Standard, 13000, 1100, 10, 24000)]
    [InlineData(PackageType.Premium, 18000, 1400, 10, 32000)]
    public void CalculatePrice_racuna_osnovnu_cenu_uvecanu_za_cenu_po_detetu(
        PackageType type, decimal basePrice, decimal pricePerChild, int children, decimal expected)
    {
        var package = Package.Create(type, "Test paket", "Opis", basePrice, pricePerChild);

        package.CalculatePrice(children).Should().Be(expected);
    }

    [Fact]
    public void UpdatePricing_ne_dozvoljava_negativnu_cenu()
    {
        var package = Package.Create(PackageType.Basic, "Test", "Opis", 9000, 900);

        var akcija = () => package.UpdatePricing(-100, 900);

        akcija.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Naknadna_izmena_cenovnika_ne_menja_vec_obracunatu_cenu_rezervacije()
    {
        // PP-08: cena se belezi na rezervaciji u trenutku kreiranja.
        var package = Package.Create(PackageType.Basic, "Test", "Opis", 9000, 900);
        var reservation = Igraonica.Domain.Entities.Reservation.Create(
            Guid.NewGuid(), "Marko", 6, new DateOnly(2026, 6, 20), Guid.NewGuid(), 10, package,
            new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero), 5);

        package.UpdatePricing(50000, 5000);

        reservation.TotalPrice.Should().Be(18000);
    }
}

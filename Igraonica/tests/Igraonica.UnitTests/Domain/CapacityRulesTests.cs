using FluentAssertions;
using Igraonica.Domain.Exceptions;
using Igraonica.Domain.Rules;
using Xunit;

namespace Igraonica.UnitTests.Domain;

public class CapacityRulesTests
{
    // T-01: prva rezervacija za radni dan - prihvacena
    [Fact]
    public void Radnim_danom_prva_rezervacija_prolazi()
    {
        var ponedeljak = new DateOnly(2026, 3, 2);

        var akcija = () => CapacityRules.EnsureCapacityAvailable(
            ponedeljak, existingCount: 0, weekdayCapacity: 1, weekendCapacity: 3);

        akcija.Should().NotThrow();
    }

    // T-02: druga rezervacija za isti radni dan - odbijena (PP-01)
    [Fact]
    public void Radnim_danom_druga_rezervacija_se_odbija()
    {
        var ponedeljak = new DateOnly(2026, 3, 2);

        var akcija = () => CapacityRules.EnsureCapacityAvailable(
            ponedeljak, existingCount: 1, weekdayCapacity: 1, weekendCapacity: 3);

        akcija.Should().Throw<CapacityExceededException>();
    }

    // T-03: tri rezervacije za istu subotu - sve prihvacene (PP-02)
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Vikendom_prve_tri_rezervacije_prolaze(int postojece)
    {
        var subota = new DateOnly(2026, 3, 7);

        var akcija = () => CapacityRules.EnsureCapacityAvailable(
            subota, postojece, weekdayCapacity: 1, weekendCapacity: 3);

        akcija.Should().NotThrow();
    }

    // T-04: cetvrta rezervacija za istu subotu - odbijena
    [Fact]
    public void Vikendom_cetvrta_rezervacija_se_odbija()
    {
        var subota = new DateOnly(2026, 3, 7);

        var akcija = () => CapacityRules.EnsureCapacityAvailable(
            subota, existingCount: 3, weekdayCapacity: 1, weekendCapacity: 3);

        akcija.Should().Throw<CapacityExceededException>();
    }

    [Theory]
    [InlineData(2026, 3, 2, false)] // ponedeljak
    [InlineData(2026, 3, 6, false)] // petak
    [InlineData(2026, 3, 7, true)]  // subota
    [InlineData(2026, 3, 8, true)]  // nedelja
    public void IsWeekend_prepoznaje_vikend(int year, int month, int day, bool ocekivano)
    {
        CapacityRules.IsWeekend(new DateOnly(year, month, day)).Should().Be(ocekivano);
    }
}

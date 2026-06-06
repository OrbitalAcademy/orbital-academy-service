using OrbitalAcademy.Domain.Areas;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class AreaDomainTests
{
    [Fact]
    public void Area_accepts_valid_documented_data()
    {
        // Given valid monitored area data.
        Guid id = Guid.Parse("bcbb2fbc-b3e5-4d28-ad6c-9ff52d31541e");

        // When creating the domain area.
        Area area = new(
            id,
            -23.5505m,
            -46.6333m,
            "Soja",
            120.5m,
            "Unidade Agro");

        // Then the area exposes normalized textual values and original coordinates.
        Assert.Equal(id, area.Id);
        Assert.Equal(-23.5505m, area.Lat);
        Assert.Equal(-46.6333m, area.Lng);
        Assert.Equal("Soja", area.Cultura);
        Assert.Equal(120.5m, area.Tamanho);
        Assert.Equal("Unidade Agro", area.Dono);
    }

    [Fact]
    public void Area_rejects_empty_id()
    {
        Assert.Throws<ArgumentException>(() => new Area(
            Guid.Empty,
            -23.5505m,
            -46.6333m,
            "Soja",
            120.5m,
            "Unidade Agro"));
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(90.1)]
    public void Area_rejects_latitude_outside_valid_range(double lat)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Area(
            Guid.Parse("bcbb2fbc-b3e5-4d28-ad6c-9ff52d31541e"),
            (decimal)lat,
            -46.6333m,
            "Soja",
            120.5m,
            "Unidade Agro"));
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(180.1)]
    public void Area_rejects_longitude_outside_valid_range(double lng)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Area(
            Guid.Parse("bcbb2fbc-b3e5-4d28-ad6c-9ff52d31541e"),
            -23.5505m,
            (decimal)lng,
            "Soja",
            120.5m,
            "Unidade Agro"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Area_rejects_empty_cultura(string cultura)
    {
        Assert.Throws<ArgumentException>(() => new Area(
            Guid.Parse("bcbb2fbc-b3e5-4d28-ad6c-9ff52d31541e"),
            -23.5505m,
            -46.6333m,
            cultura,
            120.5m,
            "Unidade Agro"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Area_rejects_empty_dono(string dono)
    {
        Assert.Throws<ArgumentException>(() => new Area(
            Guid.Parse("bcbb2fbc-b3e5-4d28-ad6c-9ff52d31541e"),
            -23.5505m,
            -46.6333m,
            "Soja",
            120.5m,
            dono));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Area_rejects_non_positive_tamanho(double tamanho)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Area(
            Guid.Parse("bcbb2fbc-b3e5-4d28-ad6c-9ff52d31541e"),
            -23.5505m,
            -46.6333m,
            "Soja",
            (decimal)tamanho,
            "Unidade Agro"));
    }
}

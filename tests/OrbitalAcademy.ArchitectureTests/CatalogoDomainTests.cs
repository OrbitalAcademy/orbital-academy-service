using OrbitalAcademy.Domain.Catalogo;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class CatalogoDomainTests
{
    [Fact]
    public void Periodo_operacional_rejects_invalid_dates()
    {
        // Given an end date before the start date.
        DateTimeOffset inicio = new(2026, 6, 4, 10, 0, 0, TimeSpan.Zero);
        DateTimeOffset fim = inicio.AddMinutes(-1);

        // When creating the period, then the invalid interval is rejected.
        Assert.Throws<ArgumentOutOfRangeException>(() => new PeriodoOperacional(inicio, fim));
    }

    [Fact]
    public void Periodo_operacional_can_be_created_from_start_and_duration()
    {
        // Given a known start date and duration.
        DateTimeOffset inicio = new(2026, 6, 4, 10, 0, 0, TimeSpan.Zero);

        // When creating the period through the factory method.
        PeriodoOperacional periodo = PeriodoOperacional.CriarComDuracao(inicio, TimeSpan.FromHours(6));

        // Then it exposes precise DateTimeOffset boundaries.
        Assert.Equal(inicio, periodo.Inicio);
        Assert.Equal(inicio.AddHours(6), periodo.Fim);
        Assert.Equal(TimeSpan.FromHours(6), periodo.Duracao);
    }

    [Fact]
    public void Alerta_de_risco_vegetacao_uses_validity_window_to_report_active_state()
    {
        // Given a vegetation risk alert with an operational validity window.
        DateTimeOffset detectadoEm = new(2026, 6, 4, 10, 0, 0, TimeSpan.Zero);
        PeriodoOperacional validade = PeriodoOperacional.CriarComDuracao(detectadoEm, TimeSpan.FromHours(12));
        AlertaRiscoVegetacao alerta = new(
            Guid.Parse("f61c2738-2765-47ad-a5e4-924d1a410017"),
            "Risco de vegetacao",
            validade);

        // When checking moments inside and outside the validity window.
        bool ativoDuranteValidade = alerta.EstaAtivoEm(detectadoEm.AddHours(3));
        bool ativoDepoisDaExpiracao = alerta.EstaAtivoEm(detectadoEm.AddHours(13));

        // Then the domain method reports the expected alert state.
        Assert.True(ativoDuranteValidade);
        Assert.False(ativoDepoisDaExpiracao);
        Assert.Equal(detectadoEm, alerta.DetectadoEm);
        Assert.Equal(detectadoEm.AddHours(12), alerta.ExpiraEm);
    }

    [Fact]
    public void Satelite_exposes_sensors_and_alerts_as_read_only_collections()
    {
        // Given a satellite with one sensor and one alert.
        Satelite satelite = new(
            Guid.Parse("fe1d9f7c-4d77-493d-8f6e-d715f4e7a3a7"),
            "Sentinel");
        SensorOptico sensor = new(
            Guid.Parse("5f2727ab-e093-4058-b82c-b2bb4e4d3276"),
            "Sensor optico");
        AlertaRiscoVegetacao alerta = new(
            Guid.Parse("bddc8790-0381-47a9-a5e1-a5d598ed4084"),
            "Risco em lavoura",
            PeriodoOperacional.CriarComDuracao(
                new DateTimeOffset(2026, 6, 4, 10, 0, 0, TimeSpan.Zero),
                TimeSpan.FromHours(6)));

        satelite.AdicionarSensor(sensor);
        satelite.AdicionarAlerta(alerta);

        // When reading the exposed collections as generic collections.
        ICollection<Sensor> sensores = Assert.IsAssignableFrom<ICollection<Sensor>>(satelite.Sensores);
        ICollection<Alerta> alertas = Assert.IsAssignableFrom<ICollection<Alerta>>(satelite.Alertas);

        // Then external callers cannot mutate the satellite catalog directly.
        Assert.True(sensores.IsReadOnly);
        Assert.True(alertas.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => sensores.Add(new SensorRadar(
            Guid.Parse("ace7f988-4cff-4e9a-a1f2-67826c465bd1"),
            "Sensor radar")));
        Assert.Throws<NotSupportedException>(() => alertas.Clear());
        Assert.Single(satelite.Sensores);
        Assert.Single(satelite.Alertas);
    }
}

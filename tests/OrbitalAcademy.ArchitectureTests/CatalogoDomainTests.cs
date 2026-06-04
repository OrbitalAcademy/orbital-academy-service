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
}

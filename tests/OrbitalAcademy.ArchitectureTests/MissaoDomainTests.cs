using OrbitalAcademy.Domain.Missoes;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class MissaoDomainTests
{
    [Theory]
    [InlineData(MissaoStatus.Identificada)]
    [InlineData(MissaoStatus.Validada)]
    [InlineData(MissaoStatus.Decidida)]
    [InlineData(MissaoStatus.EmExecucao)]
    [InlineData(MissaoStatus.Concluida)]
    [InlineData(MissaoStatus.Reprogramada)]
    [InlineData(MissaoStatus.Perdida)]
    public void Missao_accepts_documented_statuses(string status)
    {
        // Given a mission status documented in PROJETO.md.
        DateTimeOffset prazo = new(2026, 6, 9, 18, 0, 0, TimeSpan.Zero);

        // When creating the domain mission.
        Missao missao = CriarMissao(status: status, prazo: prazo);

        // Then the status is accepted without defining transition rules.
        Assert.Equal(status, missao.Status);
        Assert.Equal(prazo, missao.Prazo);
    }

    [Theory]
    [InlineData(" identificada ", MissaoStatus.Identificada)]
    [InlineData("VALIDADA", MissaoStatus.Validada)]
    [InlineData("em execucao", MissaoStatus.EmExecucao)]
    public void Missao_normalizes_status_from_simple_text_input(string status, string expectedStatus)
    {
        Missao missao = CriarMissao(status: status);

        Assert.Equal(expectedStatus, missao.Status);
    }

    [Fact]
    public void Missao_rejects_status_not_documented()
    {
        Assert.Throws<ArgumentException>(() => CriarMissao(status: "Cancelada"));
    }

    [Fact]
    public void Missao_rejects_empty_id()
    {
        Assert.Throws<ArgumentException>(() => CriarMissao(id: Guid.Empty));
    }

    [Fact]
    public void Missao_rejects_empty_area_id()
    {
        Assert.Throws<ArgumentException>(() => CriarMissao(areaId: Guid.Empty));
    }

    [Fact]
    public void Missao_rejects_empty_responsavel_id()
    {
        Assert.Throws<ArgumentException>(() => CriarMissao(responsavelId: Guid.Empty));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Missao_rejects_empty_prioridade(string prioridade)
    {
        Assert.Throws<ArgumentException>(() => CriarMissao(prioridade: prioridade));
    }

    [Fact]
    public void Missao_rejects_default_prazo()
    {
        Assert.Throws<ArgumentException>(() => CriarMissao(prazo: default(DateTimeOffset)));
    }

    private static Missao CriarMissao(
        Guid? id = null,
        Guid? areaId = null,
        string prioridade = "alta",
        string status = MissaoStatus.Identificada,
        Guid? responsavelId = null,
        DateTimeOffset? prazo = null)
    {
        return new Missao(
            id ?? Guid.Parse("91719ebb-6afa-434f-b060-18314922ee56"),
            areaId ?? Guid.Parse("11111111-1111-1111-1111-111111111111"),
            prioridade,
            status,
            responsavelId ?? Guid.Parse("22222222-2222-2222-2222-222222222222"),
            prazo ?? new DateTimeOffset(2026, 6, 9, 18, 0, 0, TimeSpan.Zero));
    }
}

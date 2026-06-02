namespace OrbitalAcademy.Api.Contracts.Otimizacao;

public sealed record OtimizarRequest(
    IReadOnlyCollection<Guid> AreaIds,
    IReadOnlyCollection<Guid> RecursoIds);

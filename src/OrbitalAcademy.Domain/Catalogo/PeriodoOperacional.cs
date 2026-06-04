namespace OrbitalAcademy.Domain.Catalogo;

public readonly record struct PeriodoOperacional
{
    public PeriodoOperacional(DateTimeOffset inicio, DateTimeOffset fim)
    {
        if (fim <= inicio)
        {
            throw new ArgumentOutOfRangeException(nameof(fim), "O fim do periodo deve ser posterior ao inicio.");
        }

        Inicio = inicio;
        Fim = fim;
    }

    public DateTimeOffset Inicio { get; }

    public DateTimeOffset Fim { get; }

    public TimeSpan Duracao => Fim - Inicio;

    public bool Contem(DateTimeOffset momento)
    {
        return momento >= Inicio && momento <= Fim;
    }

    public static PeriodoOperacional CriarComDuracao(DateTimeOffset inicio, TimeSpan duracao)
    {
        if (duracao <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duracao), "A duracao deve ser positiva.");
        }

        return new PeriodoOperacional(inicio, inicio.Add(duracao));
    }
}

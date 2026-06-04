namespace OrbitalAcademy.Domain.Catalogo;

public abstract class Alerta
{
    protected Alerta(Guid id, string nome, PeriodoOperacional validade)
    {
        Id = id == Guid.Empty ? throw new ArgumentException("O id do alerta deve ser informado.", nameof(id)) : id;
        Nome = ExigirTexto(nome, nameof(nome));
        Validade = validade;
    }

    public Guid Id { get; }

    public string Nome { get; }

    public PeriodoOperacional Validade { get; }

    public DateTimeOffset DetectadoEm => Validade.Inicio;

    public DateTimeOffset ExpiraEm => Validade.Fim;

    public abstract string Tipo { get; }

    public abstract string DescreverDisparo();

    public bool EstaAtivoEm(DateTimeOffset momento)
    {
        return Validade.Contem(momento);
    }

    protected static string ExigirTexto(string valor, string nomeParametro)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? throw new ArgumentException("O valor deve ser informado.", nomeParametro)
            : valor.Trim();
    }
}

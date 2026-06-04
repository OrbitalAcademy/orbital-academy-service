namespace OrbitalAcademy.Domain.Catalogo;

public abstract class Sensor
{
    protected Sensor(Guid id, string nome)
    {
        Id = id == Guid.Empty ? throw new ArgumentException("O id do sensor deve ser informado.", nameof(id)) : id;
        Nome = ExigirTexto(nome, nameof(nome));
    }

    public Guid Id { get; }

    public string Nome { get; }

    public abstract string Tipo { get; }

    public abstract string DescreverLeitura();

    protected static string ExigirTexto(string valor, string nomeParametro)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? throw new ArgumentException("O valor deve ser informado.", nomeParametro)
            : valor.Trim();
    }
}

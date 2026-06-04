namespace OrbitalAcademy.Domain.Catalogo;

public sealed class Satelite
{
    private readonly List<Sensor> sensores = [];
    private readonly List<Alerta> alertas = [];

    public Satelite(Guid id, string nome)
    {
        Id = id == Guid.Empty ? throw new ArgumentException("O id do satelite deve ser informado.", nameof(id)) : id;
        Nome = ExigirTexto(nome, nameof(nome));
    }

    public Guid Id { get; }

    public string Nome { get; }

    public IReadOnlyCollection<Sensor> Sensores => sensores.AsReadOnly();

    public IReadOnlyCollection<Alerta> Alertas => alertas.AsReadOnly();

    public void AdicionarSensor(Sensor sensor)
    {
        ArgumentNullException.ThrowIfNull(sensor);

        sensores.Add(sensor);
    }

    public void AdicionarAlerta(Alerta alerta)
    {
        ArgumentNullException.ThrowIfNull(alerta);

        alertas.Add(alerta);
    }

    private static string ExigirTexto(string valor, string nomeParametro)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? throw new ArgumentException("O valor deve ser informado.", nomeParametro)
            : valor.Trim();
    }
}

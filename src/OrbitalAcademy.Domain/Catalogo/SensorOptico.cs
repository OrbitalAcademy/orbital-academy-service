namespace OrbitalAcademy.Domain.Catalogo;

public sealed class SensorOptico : Sensor
{
    public SensorOptico(Guid id, string nome)
        : base(id, nome)
    {
    }

    public override string Tipo => "optico";

    public override string DescreverLeitura()
    {
        return "Leitura optica de dados espaciais.";
    }
}

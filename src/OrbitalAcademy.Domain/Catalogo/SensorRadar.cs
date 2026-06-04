namespace OrbitalAcademy.Domain.Catalogo;

public sealed class SensorRadar : Sensor
{
    public SensorRadar(Guid id, string nome)
        : base(id, nome)
    {
    }

    public override string Tipo => "radar";

    public override string DescreverLeitura()
    {
        return "Leitura por radar de dados espaciais.";
    }
}

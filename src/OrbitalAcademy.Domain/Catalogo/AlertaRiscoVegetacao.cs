namespace OrbitalAcademy.Domain.Catalogo;

public sealed class AlertaRiscoVegetacao : Alerta
{
    public AlertaRiscoVegetacao(Guid id, string nome)
        : base(id, nome)
    {
    }

    public override string Tipo => "risco-vegetacao";

    public override string DescreverDisparo()
    {
        return "Disparo de alerta para risco em vegetacao.";
    }
}

using OrbitalAcademy.Domain.Catalogo;

namespace OrbitalAcademy.Application.Catalogo;

public sealed class CatalogoSatelitesService : ICatalogoSatelitesService
{
    private static readonly DateTimeOffset DataReferencia = new(2026, 6, 4, 0, 0, 0, TimeSpan.Zero);

    public IReadOnlyCollection<SateliteCatalogoItem> ListarSatelites()
    {
        IReadOnlyCollection<Satelite> satelites =
        [
            CriarLandsat(),
            CriarSentinel()
        ];

        ExigirCatalogoComSatelites(satelites);

        return satelites.Select(MapearSatelite).ToArray();
    }

    private static void ExigirCatalogoComSatelites(IReadOnlyCollection<Satelite> satelites)
    {
        if (satelites.Count == 0)
        {
            throw new CatalogoEspacialException("O catalogo espacial deve conter pelo menos um satelite.");
        }
    }

    private static Satelite CriarLandsat()
    {
        Satelite satelite = new(
            Guid.Parse("41d9d5f0-6053-4f8f-8a5c-59bb1c55a7b9"),
            "Landsat");

        satelite.AdicionarSensor(new SensorOptico(
            Guid.Parse("a2f1c03d-222e-4c0e-b53f-48cc089a9c10"),
            "Sensor optico para vegetacao"));

        satelite.AdicionarAlerta(new AlertaRiscoVegetacao(
            Guid.Parse("22a9e33f-4127-4069-a0b4-c6e8a0c4de26"),
            "Risco em lavoura",
            PeriodoOperacional.CriarComDuracao(DataReferencia, TimeSpan.FromDays(7))));

        return satelite;
    }

    private static Satelite CriarSentinel()
    {
        Satelite satelite = new(
            Guid.Parse("c1b6a32d-889c-4c73-85af-3b8ee30f3a59"),
            "Sentinel");

        satelite.AdicionarSensor(new SensorRadar(
            Guid.Parse("fd336d99-e4c5-4f28-a940-fce2906736fa"),
            "Sensor radar para cobertura de solo"));

        satelite.AdicionarAlerta(new AlertaRiscoVegetacao(
            Guid.Parse("ec0e53ec-3963-4d26-98b4-346c08f2c05f"),
            "Risco de estresse hidrico",
            PeriodoOperacional.CriarComDuracao(DataReferencia.AddDays(1), TimeSpan.FromDays(6))));

        return satelite;
    }

    private static SateliteCatalogoItem MapearSatelite(Satelite satelite)
    {
        return new SateliteCatalogoItem(
            satelite.Id,
            satelite.Nome,
            satelite.Sensores.Select(MapearSensor).ToArray(),
            satelite.Alertas.Select(MapearAlerta).ToArray());
    }

    private static SensorCatalogoItem MapearSensor(Sensor sensor)
    {
        return new SensorCatalogoItem(
            sensor.Id,
            sensor.Nome,
            sensor.Tipo,
            sensor.DescreverLeitura());
    }

    private static AlertaCatalogoItem MapearAlerta(Alerta alerta)
    {
        return new AlertaCatalogoItem(
            alerta.Id,
            alerta.Nome,
            alerta.Tipo,
            alerta.DetectadoEm,
            alerta.ExpiraEm,
            alerta.DescreverDisparo());
    }
}

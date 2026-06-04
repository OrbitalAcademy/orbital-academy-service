namespace OrbitalAcademy.Application.Catalogo;

public interface ICatalogoSatelitesService
{
    IReadOnlyCollection<SateliteCatalogoItem> ListarSatelites();
}

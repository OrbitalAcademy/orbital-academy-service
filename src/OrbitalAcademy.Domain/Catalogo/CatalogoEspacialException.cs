namespace OrbitalAcademy.Domain.Catalogo;

public sealed class CatalogoEspacialException : Exception
{
    public CatalogoEspacialException(string message)
        : base(message)
    {
    }

    public CatalogoEspacialException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

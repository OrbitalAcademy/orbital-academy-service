namespace OrbitalAcademy.Infrastructure.Authentication;

public sealed class InitialUserOptions
{
    public const string SectionName = "Authentication:InitialUser";

    public bool Enabled { get; init; }

    public string? Email { get; init; }

    public string? Nome { get; init; }

    public string? Papel { get; init; }

    public string? Unidade { get; init; }

    public string? Password { get; init; }
}

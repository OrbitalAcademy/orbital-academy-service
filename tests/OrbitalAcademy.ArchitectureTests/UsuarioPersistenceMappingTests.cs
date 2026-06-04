using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OrbitalAcademy.Domain.Usuarios;
using OrbitalAcademy.Infrastructure.Persistence;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class UsuarioPersistenceMappingTests
{
    [Fact]
    public void Orbital_academy_db_context_exposes_usuarios_db_set()
    {
        // Given the infrastructure persistence context.
        DbContextOptions<OrbitalAcademyDbContext> options = new DbContextOptionsBuilder<OrbitalAcademyDbContext>()
            .UseNpgsql("Host=localhost;Database=orbital_academy;Username=orbital;Password=orbital")
            .Options;

        using OrbitalAcademyDbContext dbContext = new(options);

        // Then Usuario is part of the EF model and exposed by DbSet.
        Assert.NotNull(dbContext.Usuarios);
        Assert.NotNull(dbContext.Model.FindEntityType(typeof(Usuario)));
    }

    [Fact]
    public void Usuario_mapping_uses_unique_normalized_email_and_hashed_password_column()
    {
        // Given the EF model for Usuario.
        DbContextOptions<OrbitalAcademyDbContext> options = new DbContextOptionsBuilder<OrbitalAcademyDbContext>()
            .UseNpgsql("Host=localhost;Database=orbital_academy;Username=orbital;Password=orbital")
            .Options;

        using OrbitalAcademyDbContext dbContext = new(options);
        IEntityType entity = dbContext.Model.FindEntityType(typeof(Usuario))
            ?? throw new InvalidOperationException("Usuario entity was not mapped.");
        StoreObjectIdentifier table = StoreObjectIdentifier.Table("usuarios", null);

        // Then the database stores the normalized email uniquely and password only as hash.
        IProperty senhaHash = entity.FindProperty(nameof(Usuario.SenhaHash))
            ?? throw new InvalidOperationException("SenhaHash was not mapped.");
        IProperty emailNormalizado = entity.FindProperty(nameof(Usuario.EmailNormalizado))
            ?? throw new InvalidOperationException("EmailNormalizado was not mapped.");

        Assert.Equal("senha_hash", senhaHash.GetColumnName(table));
        Assert.Equal("email_normalizado", emailNormalizado.GetColumnName(table));
        Assert.Contains(entity.GetIndexes(), index =>
            index.IsUnique &&
            index.Properties.Single().Name == nameof(Usuario.EmailNormalizado));
    }
}

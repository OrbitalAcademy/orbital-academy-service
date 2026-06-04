using Microsoft.EntityFrameworkCore;
using OrbitalAcademy.Domain.Usuarios;

namespace OrbitalAcademy.Infrastructure.Persistence;

public sealed class OrbitalAcademyDbContext : DbContext
{
    public OrbitalAcademyDbContext(DbContextOptions<OrbitalAcademyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");

            entity.HasKey(usuario => usuario.Id)
                .HasName("pk_usuarios");

            entity.Property(usuario => usuario.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(usuario => usuario.Nome)
                .HasColumnName("nome")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(usuario => usuario.Email)
                .HasColumnName("email")
                .HasMaxLength(254)
                .IsRequired();

            entity.Property(usuario => usuario.EmailNormalizado)
                .HasColumnName("email_normalizado")
                .HasMaxLength(254)
                .IsRequired();

            entity.Property(usuario => usuario.SenhaHash)
                .HasColumnName("senha_hash")
                .IsRequired();

            entity.Property(usuario => usuario.Papel)
                .HasColumnName("papel")
                .HasMaxLength(32)
                .IsRequired();

            entity.Property(usuario => usuario.Unidade)
                .HasColumnName("unidade")
                .HasMaxLength(160)
                .IsRequired();

            entity.HasIndex(usuario => usuario.EmailNormalizado)
                .IsUnique()
                .HasDatabaseName("ix_usuarios_email_normalizado");
        });
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using OrbitalAcademy.Infrastructure.Persistence;

#nullable disable

namespace OrbitalAcademy.Infrastructure.Migrations;

[DbContext(typeof(OrbitalAcademyDbContext))]
partial class OrbitalAcademyDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.4")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity("OrbitalAcademy.Domain.Usuarios.Usuario", entity =>
        {
            entity.Property<Guid>("Id")
                .ValueGeneratedNever()
                .HasColumnType("uuid")
                .HasColumnName("id");

            entity.Property<string>("Email")
                .IsRequired()
                .HasMaxLength(254)
                .HasColumnType("character varying(254)")
                .HasColumnName("email");

            entity.Property<string>("EmailNormalizado")
                .IsRequired()
                .HasMaxLength(254)
                .HasColumnType("character varying(254)")
                .HasColumnName("email_normalizado");

            entity.Property<string>("Nome")
                .IsRequired()
                .HasMaxLength(160)
                .HasColumnType("character varying(160)")
                .HasColumnName("nome");

            entity.Property<string>("Papel")
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnType("character varying(32)")
                .HasColumnName("papel");

            entity.Property<string>("SenhaHash")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("senha_hash");

            entity.Property<string>("Unidade")
                .IsRequired()
                .HasMaxLength(160)
                .HasColumnType("character varying(160)")
                .HasColumnName("unidade");

            entity.HasKey("Id")
                .HasName("pk_usuarios");

            entity.HasIndex("EmailNormalizado")
                .IsUnique()
                .HasDatabaseName("ix_usuarios_email_normalizado");

            entity.ToTable("usuarios");
        });
#pragma warning restore 612, 618
    }
}

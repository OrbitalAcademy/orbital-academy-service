using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrbitalAcademy.Infrastructure.Migrations;

public partial class CreateUsuarios : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "usuarios",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                email_normalizado = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                senha_hash = table.Column<string>(type: "text", nullable: false),
                papel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                unidade = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_usuarios", usuario => usuario.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_usuarios_email_normalizado",
            table: "usuarios",
            column: "email_normalizado",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "usuarios");
    }
}

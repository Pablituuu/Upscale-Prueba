using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UpscaleWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoUrlToUsuariosDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuariosDetalle",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    NombresCompletoCabecera = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Rol = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Entidad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PrimerApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SegundoApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TipoDocumento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    Nacionalidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Sexo = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    EmailPrincipal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EmailSecundario = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TelefonoSecundario = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TelefonoMovil = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TipoContratacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaContratacion = table.Column<DateOnly>(type: "date", nullable: true),
                    FotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuarios__2B3DE7B8739602B1", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IntentosFallidos = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    BloqueadoHasta = table.Column<DateTime>(type: "datetime", nullable: true),
                    UsuarioDetalleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cuentas__3214EC0773F8C68F", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cuentas_UsuariosDetalle",
                        column: x => x.UsuarioDetalleId,
                        principalTable: "UsuariosDetalle",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_UsuarioDetalleId",
                table: "Cuentas",
                column: "UsuarioDetalleId");

            migrationBuilder.CreateIndex(
                name: "UQ__Cuentas__536C85E4108580FE",
                table: "Cuentas",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Usuarios__A4202588BABC2E16",
                table: "UsuariosDetalle",
                column: "NumeroDocumento",
                unique: true,
                filter: "[NumeroDocumento] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cuentas");

            migrationBuilder.DropTable(
                name: "UsuariosDetalle");
        }
    }
}

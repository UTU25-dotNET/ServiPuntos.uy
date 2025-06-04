using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigracionTasas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CambioNeumaticos",
                table: "Ubicaciones",
                newName: "LavadoDeAuto");

            migrationBuilder.RenameColumn(
                name: "TasaPuntos",
                table: "Tenants",
                newName: "TasaServicios");

            migrationBuilder.AddColumn<bool>(
                name: "CambioDeAceite",
                table: "Ubicaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CambioDeNeumaticos",
                table: "Ubicaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TasaCombustible",
                table: "Tenants",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TasaMinimercado",
                table: "Tenants",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CambioDeAceite",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "CambioDeNeumaticos",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "TasaCombustible",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TasaMinimercado",
                table: "Tenants");

            migrationBuilder.RenameColumn(
                name: "LavadoDeAuto",
                table: "Ubicaciones",
                newName: "CambioNeumaticos");

            migrationBuilder.RenameColumn(
                name: "TasaServicios",
                table: "Tenants",
                newName: "TasaPuntos");
        }
    }
}

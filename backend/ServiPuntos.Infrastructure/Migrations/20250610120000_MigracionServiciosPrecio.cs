using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntos.Infrastructure.Migrations
{
    public partial class MigracionServiciosPrecio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecioCambioAceite",
                table: "Ubicaciones",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioCambioNeumaticos",
                table: "Ubicaciones",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioLavado",
                table: "Ubicaciones",
                type: "numeric",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioCambioAceite",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "PrecioCambioNeumaticos",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "PrecioLavado",
                table: "Ubicaciones");
        }
    }
}

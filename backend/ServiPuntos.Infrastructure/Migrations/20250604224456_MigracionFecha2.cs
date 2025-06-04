using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigracionFecha2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaDeModificacion",
                table: "Ubicaciones",
                newName: "FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "FechaDeCreacion",
                table: "Ubicaciones",
                newName: "FechaCreacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaModificacion",
                table: "Ubicaciones",
                newName: "FechaDeModificacion");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Ubicaciones",
                newName: "FechaDeCreacion");
        }
    }
}

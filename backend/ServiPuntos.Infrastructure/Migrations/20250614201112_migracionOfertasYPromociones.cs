using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migracionOfertasYPromociones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DescuentoEnPuntos",
                table: "Promociones",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DescuentoEnPuntos",
                table: "Promociones",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}

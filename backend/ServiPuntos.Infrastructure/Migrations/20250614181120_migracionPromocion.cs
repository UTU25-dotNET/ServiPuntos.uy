using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migracionPromocion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrecioEnPuntos",
                table: "Promociones",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Promociones",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Promociones",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioEnPuntos",
                table: "Promociones");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Promociones");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Promociones");
        }
    }
}

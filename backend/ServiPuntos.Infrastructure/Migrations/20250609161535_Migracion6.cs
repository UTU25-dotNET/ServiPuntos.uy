using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migracion6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UbicacionId",
                table: "Usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_UbicacionId",
                table: "Usuarios",
                column: "UbicacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Ubicaciones_UbicacionId",
                table: "Usuarios",
                column: "UbicacionId",
                principalTable: "Ubicaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Ubicaciones_UbicacionId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_UbicacionId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UbicacionId",
                table: "Usuarios");
        }
    }
}

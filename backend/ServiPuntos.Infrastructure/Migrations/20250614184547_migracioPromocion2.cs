using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migracioPromocion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromocionProductos",
                columns: table => new
                {
                    PromocionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoCanjeableId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromocionProductos", x => new { x.PromocionId, x.ProductoCanjeableId });
                    table.ForeignKey(
                        name: "FK_PromocionProductos_ProductosCanjeables_ProductoCanjeableId",
                        column: x => x.ProductoCanjeableId,
                        principalTable: "ProductosCanjeables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromocionProductos_Promociones_PromocionId",
                        column: x => x.PromocionId,
                        principalTable: "Promociones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromocionProductos_ProductoCanjeableId",
                table: "PromocionProductos",
                column: "ProductoCanjeableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromocionProductos");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migracion3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "ReferenciaExterna",
                table: "Transacciones");

            migrationBuilder.AddColumn<string>(
                name: "EstadoPayPal",
                table: "Transacciones",
                type: "text",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompletadoPayPal",
                table: "Transacciones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoPayPal",
                table: "Transacciones",
                type: "numeric",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PagoPayPalId",
                table: "Transacciones",
                type: "text",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PayPalPayerId",
                table: "Transacciones",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PayPalToken",
                table: "Transacciones",
                type: "text",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PuntosUtilizados",
                table: "Transacciones",
                type: "integer",
                nullable: true,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoPayPal",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "FechaCompletadoPayPal",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "MontoPayPal",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "PagoPayPalId",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "PayPalPayerId",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "PayPalToken",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "PuntosUtilizados",
                table: "Transacciones");

            migrationBuilder.AddColumn<bool>(
                name: "CambioAceite",
                table: "Ubicaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Telefonoo",
                table: "Ubicaciones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenciaExterna",
                table: "Transacciones",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}

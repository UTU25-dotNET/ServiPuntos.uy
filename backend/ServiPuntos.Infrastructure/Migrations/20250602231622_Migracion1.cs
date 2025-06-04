using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migracion1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "Bloqueado",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CiudadResidencia",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CombustiblePreferido",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EsSubscriptorPremium",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "GastoPromedio",
                table: "Usuarios",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GastoTotal",
                table: "Usuarios",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "IntentosFallidos",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<List<string>>(
                name: "Intereses",
                table: "Usuarios",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PuntosUtilizados",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SegmentoClientes",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SegmentoDinamicoId",
                table: "Usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalCompras",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalVisitas",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UbicacionPreferida",
                table: "Usuarios",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "UltimaCategoriaComprada",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaVisita",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VerificadoVEAI",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "VisitasPorMes",
                table: "Usuarios",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "CambioNeumaticos",
                table: "Ubicaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DiasCaducidadPuntos",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NombrePuntos",
                table: "Tenants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TasaPuntos",
                table: "Tenants",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "AudienciaId",
                table: "Promociones",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Audiencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    NombreUnicoInterno = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NombreDescriptivo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Prioridad = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audiencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audiencias_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Canjes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    UbicacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoCanjeableId = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoQR = table.Column<string>(type: "text", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaCanje = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    PuntosCanjeados = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canjes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Canjes_ProductosCanjeables_ProductoCanjeableId",
                        column: x => x.ProductoCanjeableId,
                        principalTable: "ProductosCanjeables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Canjes_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Canjes_Ubicaciones_UbicacionId",
                        column: x => x.UbicacionId,
                        principalTable: "Ubicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Canjes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfigPlataformas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaximoIntentosLogin = table.Column<int>(type: "integer", nullable: false),
                    TiempoExpiracion = table.Column<int>(type: "integer", nullable: false),
                    LargoMinimoPassword = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigPlataformas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transacciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    UbicacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaTransaccion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoTransaccion = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    PuntosOtorgados = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: true),
                    Detalles = table.Column<string>(type: "text", nullable: false),
                    ReferenciaExterna = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacciones_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transacciones_Ubicaciones_UbicacionId",
                        column: x => x.UbicacionId,
                        principalTable: "Ubicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transacciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReglasAudiencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AudienciaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Propiedad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Operador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Valor = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OperadorLogicoConSiguiente = table.Column<string>(type: "text", nullable: true),
                    OrdenEvaluacion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReglasAudiencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReglasAudiencia_Audiencias_AudienciaId",
                        column: x => x.AudienciaId,
                        principalTable: "Audiencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_AudienciaId",
                table: "Promociones",
                column: "AudienciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Audiencias_TenantId",
                table: "Audiencias",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Canjes_CodigoQR",
                table: "Canjes",
                column: "CodigoQR",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Canjes_ProductoCanjeableId",
                table: "Canjes",
                column: "ProductoCanjeableId");

            migrationBuilder.CreateIndex(
                name: "IX_Canjes_TenantId",
                table: "Canjes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Canjes_UbicacionId",
                table: "Canjes",
                column: "UbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Canjes_UsuarioId",
                table: "Canjes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ReglasAudiencia_AudienciaId",
                table: "ReglasAudiencia",
                column: "AudienciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_FechaTransaccion",
                table: "Transacciones",
                column: "FechaTransaccion");

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_TenantId",
                table: "Transacciones",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_UbicacionId",
                table: "Transacciones",
                column: "UbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_UsuarioId",
                table: "Transacciones",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promociones_Audiencias_AudienciaId",
                table: "Promociones",
                column: "AudienciaId",
                principalTable: "Audiencias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promociones_Audiencias_AudienciaId",
                table: "Promociones");

            migrationBuilder.DropTable(
                name: "Canjes");

            migrationBuilder.DropTable(
                name: "ConfigPlataformas");

            migrationBuilder.DropTable(
                name: "ReglasAudiencia");

            migrationBuilder.DropTable(
                name: "Transacciones");

            migrationBuilder.DropTable(
                name: "Audiencias");

            migrationBuilder.DropIndex(
                name: "IX_Promociones_AudienciaId",
                table: "Promociones");

            migrationBuilder.DropColumn(
                name: "Bloqueado",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CiudadResidencia",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CombustiblePreferido",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EsSubscriptorPremium",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "GastoPromedio",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "GastoTotal",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "IntentosFallidos",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Intereses",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PuntosUtilizados",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "SegmentoClientes",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "SegmentoDinamicoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TotalCompras",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TotalVisitas",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UbicacionPreferida",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UltimaCategoriaComprada",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UltimaVisita",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "VerificadoVEAI",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "VisitasPorMes",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CambioNeumaticos",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "DiasCaducidadPuntos",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "NombrePuntos",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TasaPuntos",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "AudienciaId",
                table: "Promociones");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}

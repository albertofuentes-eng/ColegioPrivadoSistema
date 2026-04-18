using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColegioPrivado.Migrations
{
    /// <inheritdoc />
    public partial class ExtenderComprasYDevoluciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsOfertado",
                table: "DetalleCompra",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NumeroLote",
                table: "DetalleCompra",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProductoPendiente",
                columns: table => new
                {
                    ProductoPendienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    CantidadProcesada = table.Column<int>(type: "int", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoPendiente", x => x.ProductoPendienteId);
                    table.ForeignKey(
                        name: "FK_ProductoPendiente_Compra_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compra",
                        principalColumn: "CompraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoPendiente_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoPendiente_CompraId",
                table: "ProductoPendiente",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoPendiente_ProductoId",
                table: "ProductoPendiente",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoPendiente");

            migrationBuilder.DropColumn(
                name: "EsOfertado",
                table: "DetalleCompra");

            migrationBuilder.DropColumn(
                name: "NumeroLote",
                table: "DetalleCompra");
        }
    }
}

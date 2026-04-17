using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColegioPrivado.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresaIdSoloDevolucion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "DevolucionCompras",
                type: "int",
                nullable: false,
                defaultValue: 0);     
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Proveedor");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Compra");
        }
    }
}

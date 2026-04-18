using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColegioPrivado.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCodigoLote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoLote",
                table: "Producto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoLote",
                table: "Producto");
        }
    }
}

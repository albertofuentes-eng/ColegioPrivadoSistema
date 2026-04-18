using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColegioPrivado.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCostoEnvio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostoEnvio",
                table: "Compra",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostoEnvio",
                table: "Compra");
        }
    }
}

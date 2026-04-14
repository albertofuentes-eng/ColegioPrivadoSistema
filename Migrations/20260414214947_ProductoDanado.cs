using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColegioPrivado.Migrations
{
    /// <inheritdoc />
    public partial class ProductoDanado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Danado",
                table: "DetalleDevoluciones",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Danado",
                table: "DetalleDevoluciones");
        }
    }
}

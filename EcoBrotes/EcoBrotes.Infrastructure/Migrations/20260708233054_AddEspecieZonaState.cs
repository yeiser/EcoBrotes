using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoBrotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEspecieZonaState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "ZonaUrbanaEntity",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalInscritos",
                table: "JornadaReforestacion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "EspecieArboreaEntity",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "ZonaUrbanaEntity");

            migrationBuilder.DropColumn(
                name: "TotalInscritos",
                table: "JornadaReforestacion");

            migrationBuilder.DropColumn(
                name: "State",
                table: "EspecieArboreaEntity");
        }
    }
}

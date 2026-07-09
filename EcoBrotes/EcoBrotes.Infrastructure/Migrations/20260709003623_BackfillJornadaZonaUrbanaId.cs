using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoBrotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillJornadaZonaUrbanaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill de datos historicos: las jornadas creadas antes de corregir
            // el JornadaFactory quedaron con ZonaUrbanaId vacio. Se copia el valor
            // desde ZonaId (la FK real, siempre correcta).
            migrationBuilder.Sql(
                "UPDATE \"JornadaReforestacion\" " +
                "SET \"ZonaUrbanaId\" = \"ZonaId\" " +
                "WHERE \"ZonaUrbanaId\" = '00000000-0000-0000-0000-000000000000';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Backfill de datos irreversible; no se revierte el valor.
        }
    }
}

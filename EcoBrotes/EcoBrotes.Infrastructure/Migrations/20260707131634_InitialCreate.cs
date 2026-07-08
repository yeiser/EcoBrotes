using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoBrotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TypeCustomer = table.Column<int>(type: "smallint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EspecieArboreaEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ScientificName = table.Column<string>(type: "text", nullable: false),
                    MaxHeightMeters = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspecieArboreaEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ApplyIva = table.Column<bool>(type: "boolean", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZonaUrbanaEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonaUrbanaEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JornadaReforestacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ZonaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TreeMeta = table.Column<int>(type: "integer", nullable: false),
                    VolunteerCapacity = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "smallint", nullable: false),
                    CodigoUnico = table.Column<string>(type: "text", nullable: false),
                    ZonaUrbanaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JornadaReforestacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JornadaReforestacion_ZonaUrbanaEntity_ZonaId",
                        column: x => x.ZonaId,
                        principalTable: "ZonaUrbanaEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleArbolJornada",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JornadaReforestacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EspecieArboreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleArbolJornada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleArbolJornada_EspecieArboreaEntity_EspecieArboreaId",
                        column: x => x.EspecieArboreaId,
                        principalTable: "EspecieArboreaEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleArbolJornada_JornadaReforestacion_JornadaReforestaci~",
                        column: x => x.JornadaReforestacionId,
                        principalTable: "JornadaReforestacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleArbolJornada_EspecieArboreaId",
                table: "DetalleArbolJornada",
                column: "EspecieArboreaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleArbolJornada_JornadaReforestacionId",
                table: "DetalleArbolJornada",
                column: "JornadaReforestacionId");

            migrationBuilder.CreateIndex(
                name: "IX_JornadaReforestacion_ZonaId",
                table: "JornadaReforestacion",
                column: "ZonaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "DetalleArbolJornada");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "EspecieArboreaEntity");

            migrationBuilder.DropTable(
                name: "JornadaReforestacion");

            migrationBuilder.DropTable(
                name: "ZonaUrbanaEntity");
        }
    }
}

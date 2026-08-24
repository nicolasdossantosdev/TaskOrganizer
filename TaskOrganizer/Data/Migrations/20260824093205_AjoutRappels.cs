using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskOrganizer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjoutRappels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rappels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TacheId = table.Column<int>(type: "INTEGER", nullable: false),
                    Offset = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DateHeureRappel = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Declenche = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rappels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rappels_Taches_TacheId",
                        column: x => x.TacheId,
                        principalTable: "Taches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rappels_Declenche_DateHeureRappel",
                table: "Rappels",
                columns: new[] { "Declenche", "DateHeureRappel" });

            migrationBuilder.CreateIndex(
                name: "IX_Rappels_TacheId",
                table: "Rappels",
                column: "TacheId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rappels");
        }
    }
}

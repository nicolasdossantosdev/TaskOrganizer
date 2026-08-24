using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskOrganizer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjoutCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categorie",
                table: "Taches");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TacheCategories",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    TachesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TacheCategories", x => new { x.CategoriesId, x.TachesId });
                    table.ForeignKey(
                        name: "FK_TacheCategories_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TacheCategories_Taches_TachesId",
                        column: x => x.TachesId,
                        principalTable: "Taches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Nom",
                table: "Categories",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TacheCategories_TachesId",
                table: "TacheCategories",
                column: "TachesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TacheCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "Categorie",
                table: "Taches",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }
    }
}

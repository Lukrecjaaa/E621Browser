using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E621Browser.Migrations
{
    /// <inheritdoc />
    public partial class init_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artworks_AspNetUsers_UserId1",
                table: "Artworks");

            migrationBuilder.DropIndex(
                name: "IX_Artworks_UserId1",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Artworks");

            migrationBuilder.CreateTable(
                name: "SavedArtworks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    artworkId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedArtworks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedArtworks_Artworks_artworkId",
                        column: x => x.artworkId,
                        principalTable: "Artworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedArtworks_artworkId",
                table: "SavedArtworks",
                column: "artworkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedArtworks");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Artworks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Artworks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artworks_UserId1",
                table: "Artworks",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Artworks_AspNetUsers_UserId1",
                table: "Artworks",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

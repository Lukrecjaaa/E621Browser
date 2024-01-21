using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E621Browser.Migrations
{
    /// <inheritdoc />
    public partial class init_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArtId",
                table: "SavedArtworks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArtId",
                table: "Artworks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtId",
                table: "SavedArtworks");

            migrationBuilder.DropColumn(
                name: "ArtId",
                table: "Artworks");
        }
    }
}

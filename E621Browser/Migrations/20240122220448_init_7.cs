using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E621Browser.Migrations
{
    /// <inheritdoc />
    public partial class init_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Artworks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Artworks");
        }
    }
}

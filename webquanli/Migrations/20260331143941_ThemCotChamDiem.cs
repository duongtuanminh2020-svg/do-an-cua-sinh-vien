using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class ThemCotChamDiem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Diem",
                table: "BaoCaos",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NhanXet",
                table: "BaoCaos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diem",
                table: "BaoCaos");

            migrationBuilder.DropColumn(
                name: "NhanXet",
                table: "BaoCaos");
        }
    }
}

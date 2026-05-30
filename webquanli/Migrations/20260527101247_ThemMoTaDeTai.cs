using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class ThemMoTaDeTai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LyDoTuChoi",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTaDeTai",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThaiDeTai",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LyDoTuChoi",
                table: "SinhViens");

            migrationBuilder.DropColumn(
                name: "MoTaDeTai",
                table: "SinhViens");

            migrationBuilder.DropColumn(
                name: "TrangThaiDeTai",
                table: "SinhViens");
        }
    }
}

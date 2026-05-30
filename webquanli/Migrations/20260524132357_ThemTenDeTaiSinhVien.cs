using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class ThemTenDeTaiSinhVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenDeTai",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenDeTai",
                table: "SinhViens");
        }
    }
}

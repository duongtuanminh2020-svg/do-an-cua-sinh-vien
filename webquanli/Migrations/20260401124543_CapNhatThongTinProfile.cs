using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class CapNhatThongTinProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nganh",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SDT",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "GiangViens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SDT",
                table: "GiangViens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "SinhViens");

            migrationBuilder.DropColumn(
                name: "Nganh",
                table: "SinhViens");

            migrationBuilder.DropColumn(
                name: "SDT",
                table: "SinhViens");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "GiangViens");

            migrationBuilder.DropColumn(
                name: "SDT",
                table: "GiangViens");
        }
    }
}

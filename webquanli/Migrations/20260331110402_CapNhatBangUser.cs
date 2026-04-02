using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class CapNhatBangUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GiangVienId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SinhVienId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GiangVienId",
                table: "Users",
                column: "GiangVienId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SinhVienId",
                table: "Users",
                column: "SinhVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_GiangViens_GiangVienId",
                table: "Users",
                column: "GiangVienId",
                principalTable: "GiangViens",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SinhViens_SinhVienId",
                table: "Users",
                column: "SinhVienId",
                principalTable: "SinhViens",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_GiangViens_GiangVienId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_SinhViens_SinhVienId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GiangVienId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SinhVienId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GiangVienId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SinhVienId",
                table: "Users");
        }
    }
}

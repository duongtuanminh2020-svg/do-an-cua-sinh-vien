using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class FixFKTienDo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TienDos_DangKyDeTais_DangKyDeTaiId",
                table: "TienDos");

            migrationBuilder.DropIndex(
                name: "IX_TienDos_DangKyDeTaiId",
                table: "TienDos");

            migrationBuilder.DropColumn(
                name: "DangKyDeTaiId",
                table: "TienDos");

            migrationBuilder.CreateIndex(
                name: "IX_TienDos_DangKyId",
                table: "TienDos",
                column: "DangKyId");

            migrationBuilder.AddForeignKey(
                name: "FK_TienDos_DangKyDeTais_DangKyId",
                table: "TienDos",
                column: "DangKyId",
                principalTable: "DangKyDeTais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TienDos_DangKyDeTais_DangKyId",
                table: "TienDos");

            migrationBuilder.DropIndex(
                name: "IX_TienDos_DangKyId",
                table: "TienDos");

            migrationBuilder.AddColumn<int>(
                name: "DangKyDeTaiId",
                table: "TienDos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TienDos_DangKyDeTaiId",
                table: "TienDos",
                column: "DangKyDeTaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_TienDos_DangKyDeTais_DangKyDeTaiId",
                table: "TienDos",
                column: "DangKyDeTaiId",
                principalTable: "DangKyDeTais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

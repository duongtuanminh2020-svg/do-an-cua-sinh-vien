using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DangKyDeTaiId",
                table: "TienDos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DangKyDeTaiId",
                table: "BaoCaos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TienDos_DangKyDeTaiId",
                table: "TienDos",
                column: "DangKyDeTaiId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyDeTais_DeTaiId",
                table: "DangKyDeTais",
                column: "DeTaiId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyDeTais_SinhVienId",
                table: "DangKyDeTais",
                column: "SinhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaos_DangKyDeTaiId",
                table: "BaoCaos",
                column: "DangKyDeTaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaoCaos_DangKyDeTais_DangKyDeTaiId",
                table: "BaoCaos",
                column: "DangKyDeTaiId",
                principalTable: "DangKyDeTais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyDeTais_DeTais_DeTaiId",
                table: "DangKyDeTais",
                column: "DeTaiId",
                principalTable: "DeTais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyDeTais_SinhViens_SinhVienId",
                table: "DangKyDeTais",
                column: "SinhVienId",
                principalTable: "SinhViens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TienDos_DangKyDeTais_DangKyDeTaiId",
                table: "TienDos",
                column: "DangKyDeTaiId",
                principalTable: "DangKyDeTais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaoCaos_DangKyDeTais_DangKyDeTaiId",
                table: "BaoCaos");

            migrationBuilder.DropForeignKey(
                name: "FK_DangKyDeTais_DeTais_DeTaiId",
                table: "DangKyDeTais");

            migrationBuilder.DropForeignKey(
                name: "FK_DangKyDeTais_SinhViens_SinhVienId",
                table: "DangKyDeTais");

            migrationBuilder.DropForeignKey(
                name: "FK_TienDos_DangKyDeTais_DangKyDeTaiId",
                table: "TienDos");

            migrationBuilder.DropIndex(
                name: "IX_TienDos_DangKyDeTaiId",
                table: "TienDos");

            migrationBuilder.DropIndex(
                name: "IX_DangKyDeTais_DeTaiId",
                table: "DangKyDeTais");

            migrationBuilder.DropIndex(
                name: "IX_DangKyDeTais_SinhVienId",
                table: "DangKyDeTais");

            migrationBuilder.DropIndex(
                name: "IX_BaoCaos_DangKyDeTaiId",
                table: "BaoCaos");

            migrationBuilder.DropColumn(
                name: "DangKyDeTaiId",
                table: "TienDos");

            migrationBuilder.DropColumn(
                name: "DangKyDeTaiId",
                table: "BaoCaos");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class ThemBangDotDoAn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaoCaos_DotDoAn_DotDoAnId",
                table: "BaoCaos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DotDoAn",
                table: "DotDoAn");

            migrationBuilder.RenameTable(
                name: "DotDoAn",
                newName: "DotDoAns");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DotDoAns",
                table: "DotDoAns",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaoCaos_DotDoAns_DotDoAnId",
                table: "BaoCaos",
                column: "DotDoAnId",
                principalTable: "DotDoAns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaoCaos_DotDoAns_DotDoAnId",
                table: "BaoCaos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DotDoAns",
                table: "DotDoAns");

            migrationBuilder.RenameTable(
                name: "DotDoAns",
                newName: "DotDoAn");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DotDoAn",
                table: "DotDoAn",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaoCaos_DotDoAn_DotDoAnId",
                table: "BaoCaos",
                column: "DotDoAnId",
                principalTable: "DotDoAn",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DotDoAnId",
                table: "BaoCaos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DotDoAn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotDoAn", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaos_DotDoAnId",
                table: "BaoCaos",
                column: "DotDoAnId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaoCaos_DotDoAn_DotDoAnId",
                table: "BaoCaos",
                column: "DotDoAnId",
                principalTable: "DotDoAn",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaoCaos_DotDoAn_DotDoAnId",
                table: "BaoCaos");

            migrationBuilder.DropTable(
                name: "DotDoAn");

            migrationBuilder.DropIndex(
                name: "IX_BaoCaos_DotDoAnId",
                table: "BaoCaos");

            migrationBuilder.DropColumn(
                name: "DotDoAnId",
                table: "BaoCaos");
        }
    }
}

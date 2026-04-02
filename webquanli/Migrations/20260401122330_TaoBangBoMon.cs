using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class TaoBangBoMon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoMonId",
                table: "GiangViens",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BoMons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenBoMon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoMons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GiangViens_BoMonId",
                table: "GiangViens",
                column: "BoMonId");

            migrationBuilder.AddForeignKey(
                name: "FK_GiangViens_BoMons_BoMonId",
                table: "GiangViens",
                column: "BoMonId",
                principalTable: "BoMons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiangViens_BoMons_BoMonId",
                table: "GiangViens");

            migrationBuilder.DropTable(
                name: "BoMons");

            migrationBuilder.DropIndex(
                name: "IX_GiangViens_BoMonId",
                table: "GiangViens");

            migrationBuilder.DropColumn(
                name: "BoMonId",
                table: "GiangViens");
        }
    }
}

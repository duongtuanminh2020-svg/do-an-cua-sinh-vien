using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webquanli.Migrations
{
    /// <inheritdoc />
    public partial class AddBoMonRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoMonId",
                table: "SinhViens",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_BoMonId",
                table: "SinhViens",
                column: "BoMonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SinhViens_BoMons_BoMonId",
                table: "SinhViens",
                column: "BoMonId",
                principalTable: "BoMons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SinhViens_BoMons_BoMonId",
                table: "SinhViens");

            migrationBuilder.DropIndex(
                name: "IX_SinhViens_BoMonId",
                table: "SinhViens");

            migrationBuilder.DropColumn(
                name: "BoMonId",
                table: "SinhViens");
        }
    }
}

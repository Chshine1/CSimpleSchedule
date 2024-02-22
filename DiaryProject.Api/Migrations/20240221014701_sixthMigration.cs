using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiaryProject.Api.Migrations
{
    /// <inheritdoc />
    public partial class sixthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Memo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Memo_UserId",
                table: "Memo",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Memo_User_UserId",
                table: "Memo",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memo_User_UserId",
                table: "Memo");

            migrationBuilder.DropIndex(
                name: "IX_Memo_UserId",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Memo");
        }
    }
}

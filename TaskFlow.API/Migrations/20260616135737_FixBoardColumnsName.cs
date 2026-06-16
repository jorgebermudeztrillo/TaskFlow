using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.API.Migrations
{
    /// <inheritdoc />
    public partial class FixBoardColumnsName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardColums_Projects_ProjectId",
                table: "BoardColums");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_BoardColums_BoardColumnId",
                table: "TaskItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardColums",
                table: "BoardColums");

            migrationBuilder.RenameTable(
                name: "BoardColums",
                newName: "BoardColumns");

            migrationBuilder.RenameIndex(
                name: "IX_BoardColums_ProjectId",
                table: "BoardColumns",
                newName: "IX_BoardColumns_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardColumns",
                table: "BoardColumns",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardColumns_Projects_ProjectId",
                table: "BoardColumns",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_BoardColumns_BoardColumnId",
                table: "TaskItems",
                column: "BoardColumnId",
                principalTable: "BoardColumns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardColumns_Projects_ProjectId",
                table: "BoardColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_BoardColumns_BoardColumnId",
                table: "TaskItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardColumns",
                table: "BoardColumns");

            migrationBuilder.RenameTable(
                name: "BoardColumns",
                newName: "BoardColums");

            migrationBuilder.RenameIndex(
                name: "IX_BoardColumns_ProjectId",
                table: "BoardColums",
                newName: "IX_BoardColums_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardColums",
                table: "BoardColums",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardColums_Projects_ProjectId",
                table: "BoardColums",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_BoardColums_BoardColumnId",
                table: "TaskItems",
                column: "BoardColumnId",
                principalTable: "BoardColums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

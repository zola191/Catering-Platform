using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catering.Platform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dish_category_CategoryId",
                table: "dish");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dish",
                table: "dish");

            migrationBuilder.DropPrimaryKey(
                name: "PK_category",
                table: "category");

            migrationBuilder.RenameTable(
                name: "dish",
                newName: "Dish");

            migrationBuilder.RenameTable(
                name: "category",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_dish_CategoryId",
                table: "Dish",
                newName: "IX_Dish_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dish",
                table: "Dish",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dish_Category_CategoryId",
                table: "Dish",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dish_Category_CategoryId",
                table: "Dish");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dish",
                table: "Dish");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "Dish",
                newName: "dish");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "category");

            migrationBuilder.RenameIndex(
                name: "IX_Dish_CategoryId",
                table: "dish",
                newName: "IX_dish_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dish",
                table: "dish",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_category",
                table: "category",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_dish_category_CategoryId",
                table: "dish",
                column: "CategoryId",
                principalTable: "category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catering.Platform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameCompanyIdColumnsForSupplierAndCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Users",
                newName: "Supplier_CompanyId");

            migrationBuilder.RenameColumn(
                name: "UpdayedAt",
                table: "Users",
                newName: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Supplier_CompanyId",
                table: "Users",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Users",
                newName: "UpdayedAt");
        }
    }
}

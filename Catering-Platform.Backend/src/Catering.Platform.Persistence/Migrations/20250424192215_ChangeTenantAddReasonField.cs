using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catering.Platform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTenantAddReasonField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockReason",
                table: "Tenant",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockReason",
                table: "Tenant");
        }
    }
}

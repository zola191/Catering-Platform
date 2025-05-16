using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Catering.Platform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchVectorToAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Address",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "russian")
                .Annotation("Npgsql:TsVectorProperties", new[] { "City", "StreetAndBuilding" });

            migrationBuilder.CreateIndex(
                name: "IX_Address_SearchVector",
                table: "Address",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Address_SearchVector",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "Address");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webapp.Migrations
{
    /// <inheritdoc />
    public partial class SeedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tournaments",
                column: "Name",
                values: new object[]
                {
                    "Tournament 1",
                    "Tournament 2",
                    "Tournament 3"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tournaments",
                keyColumn: "Name",
                keyValue: "Tournament 1");

            migrationBuilder.DeleteData(
                table: "Tournaments",
                keyColumn: "Name",
                keyValue: "Tournament 2");

            migrationBuilder.DeleteData(
                table: "Tournaments",
                keyColumn: "Name",
                keyValue: "Tournament 3");
        }
    }
}

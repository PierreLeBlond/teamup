using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Webapp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTournamentUserForeignKeyToName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Tournaments",
                newName: "OwnerName");

            migrationBuilder.InsertData(
                table: "Tournaments",
                columns: new[] { "Id", "Name", "OwnerName" },
                values: new object[] { new Guid("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a0a0"), "The Great Olympiad", "pierre.lespingal@gmail.com" });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Name", "NumberOfTeams", "ShouldMaximizeScore", "TournamentId" },
                values: new object[,]
                {
                    { new Guid("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c0c0"), "Hide and Seek", 2, true, new Guid("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a0a0") },
                    { new Guid("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"), "Red lights, Green lights", 2, true, new Guid("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a0a0") }
                });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "Name", "TournamentId" },
                values: new object[,]
                {
                    { new Guid("b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b0b0"), "Antigone", new Guid("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a0a0") },
                    { new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), "Bellerophon", new Guid("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a0a0") },
                    { new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), "Nausica", new Guid("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a0a0") },
                    { new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), "Achilles", new Guid("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a0a0") }
                });

            migrationBuilder.InsertData(
                table: "Rewards",
                columns: new[] { "Id", "GameId", "Value" },
                values: new object[,]
                {
                    { new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c0c0"), 200 },
                    { new Guid("f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1"), new Guid("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c0c0"), 100 },
                    { new Guid("f2f2f2f2-f2f2-f2f2-f2f2-f2f2f2f2f2f2"), new Guid("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"), 400 },
                    { new Guid("f3f3f3f3-f3f3-f3f3-f3f3-f3f3f3f3f3f3"), new Guid("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"), 200 }
                });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Bonus", "GameId", "Malus", "Number" },
                values: new object[,]
                {
                    { new Guid("d0d0d0d0-d0d0-d0d0-d0d0-d0d0d0d0d0d0"), 0, new Guid("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c0c0"), 50, 1 },
                    { new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1"), 0, new Guid("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c0c0"), 0, 2 },
                    { new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2"), 100, new Guid("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"), 0, 1 },
                    { new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3"), 0, new Guid("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"), 0, 2 }
                });

            migrationBuilder.InsertData(
                table: "Results",
                columns: new[] { "Id", "TeamId", "Value" },
                values: new object[,]
                {
                    { new Guid("e0e0e0e0-e0e0-e0e0-e0e0-e0e0e0e0e0e0"), new Guid("d0d0d0d0-d0d0-d0d0-d0d0-d0d0d0d0d0d0"), 24 },
                    { new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"), new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1"), 16 },
                    { new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"), new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2"), 3 },
                    { new Guid("e3e3e3e3-e3e3-e3e3-e3e3-e3e3e3e3e3e3"), new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3"), 4 }
                });

            migrationBuilder.InsertData(
                table: "Teammates",
                columns: new[] { "Id", "Bonus", "Malus", "PlayerId", "TeamId" },
                values: new object[,]
                {
                    { new Guid("aaaa0000-0000-0000-0000-000000000000"), 10, 0, new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), new Guid("d0d0d0d0-d0d0-d0d0-d0d0-d0d0d0d0d0d0") },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000000"), 0, 0, new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2") },
                    { new Guid("bbbb0000-0000-0000-0000-000000000000"), 0, 20, new Guid("b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b0b0"), new Guid("d0d0d0d0-d0d0-d0d0-d0d0-d0d0d0d0d0d0") },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000000"), 0, 5, new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3") },
                    { new Guid("cccc0000-0000-0000-0000-000000000000"), 0, 0, new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1") },
                    { new Guid("dddd0000-0000-0000-0000-000000000000"), 0, 0, new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1") },
                    { new Guid("eeee0000-0000-0000-0000-000000000000"), 0, 0, new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2") },
                    { new Guid("ffff0000-0000-0000-0000-000000000000"), 10, 0, new Guid("b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b0b0"), new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Results",
                keyColumn: "Id",
                keyValue: new Guid("e0e0e0e0-e0e0-e0e0-e0e0-e0e0e0e0e0e0"));

            migrationBuilder.DeleteData(
                table: "Results",
                keyColumn: "Id",
                keyValue: new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"));

            migrationBuilder.DeleteData(
                table: "Results",
                keyColumn: "Id",
                keyValue: new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"));

            migrationBuilder.DeleteData(
                table: "Results",
                keyColumn: "Id",
                keyValue: new Guid("e3e3e3e3-e3e3-e3e3-e3e3-e3e3e3e3e3e3"));

            migrationBuilder.DeleteData(
                table: "Rewards",
                keyColumn: "Id",
                keyValue: new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"));

            migrationBuilder.DeleteData(
                table: "Rewards",
                keyColumn: "Id",
                keyValue: new Guid("f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1"));

            migrationBuilder.DeleteData(
                table: "Rewards",
                keyColumn: "Id",
                keyValue: new Guid("f2f2f2f2-f2f2-f2f2-f2f2-f2f2f2f2f2f2"));

            migrationBuilder.DeleteData(
                table: "Rewards",
                keyColumn: "Id",
                keyValue: new Guid("f3f3f3f3-f3f3-f3f3-f3f3-f3f3f3f3f3f3"));

            migrationBuilder.DeleteData(
                table: "Teammates",
                keyColumn: "Id",
                keyValue: new Guid("aaaa0000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Teammates",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Teammates",
                keyColumn: "Id",
                keyValue: new Guid("bbbb0000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Teammates",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Teammates",
                keyColumn: "Id",
                keyValue: new Guid("cccc0000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Teammates",
                keyColumn: "Id",
                keyValue: new Guid("dddd0000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Teammates",
                keyColumn: "Id",
                keyValue: new Guid("eeee0000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Teammates",
                keyColumn: "Id",
                keyValue: new Guid("ffff0000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: new Guid("b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b0b0"));

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"));

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"));

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"));

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: new Guid("d0d0d0d0-d0d0-d0d0-d0d0-d0d0d0d0d0d0"));

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1"));

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2"));

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c0c0"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"));

            migrationBuilder.DeleteData(
                table: "Tournaments",
                keyColumn: "Id",
                keyValue: new Guid("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a0a0"));

            migrationBuilder.RenameColumn(
                name: "OwnerName",
                table: "Tournaments",
                newName: "OwnerId");
        }
    }
}

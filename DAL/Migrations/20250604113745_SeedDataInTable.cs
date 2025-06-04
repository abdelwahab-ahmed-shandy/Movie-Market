using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataInTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "BlockedBy", "BlockedDateUtc", "CreatedBy", "CreatedDateUtc", "CurrentState", "DeletedBy", "DeletedDateUtc", "Description", "IsDeleted", "LastAction", "Name", "RestoredBy", "RestoredDateUtc", "UpdatedBy", "UpdatedDateUtc" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0002-000000000002"), null, null, "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, null, "Anime for mature audiences.", false, "Seed Data", "Seinen", null, null, null, null },
                    { new Guid("00000000-0000-0000-0003-000000000001"), null, null, "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, null, "Anime filled with action and adventure.", false, "Seed Data", "Shonen", null, null, null, null },
                    { new Guid("00000000-0000-0000-0020-000000000003"), null, null, "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, null, "Anime with magical and supernatural elements.", false, "Seed Data", "Fantasy", null, null, null, null },
                    { new Guid("00000000-0000-0000-0030-000000000004"), null, null, "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, null, "Futuristic anime with advanced technology.", false, "Seed Data", "Sci-Fi", null, null, null, null }
                });

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000201"),
                column: "CategoryId",
                value: new Guid("00000000-0000-0000-0020-000000000003"));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000202"),
                column: "CategoryId",
                value: new Guid("00000000-0000-0000-0020-000000000003"));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000203"),
                column: "CategoryId",
                value: new Guid("00000000-0000-0000-0030-000000000004"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000001"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0020-000000000003"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0030-000000000004"));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "BlockedBy", "BlockedDateUtc", "CreatedBy", "CreatedDateUtc", "CurrentState", "DeletedBy", "DeletedDateUtc", "Description", "IsDeleted", "LastAction", "Name", "RestoredBy", "RestoredDateUtc", "UpdatedBy", "UpdatedDateUtc" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), null, null, "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, null, "Anime filled with action and adventure.", false, "Seed Data", "Shonen", null, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000002"), null, null, "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, null, "Anime for mature audiences.", false, "Seed Data", "Seinen", null, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000003"), null, null, "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, null, "Anime with magical and supernatural elements.", false, "Seed Data", "Fantasy", null, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000004"), null, null, "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, null, "Futuristic anime with advanced technology.", false, "Seed Data", "Sci-Fi", null, null, null, null }
                });

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000201"),
                column: "CategoryId",
                value: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000202"),
                column: "CategoryId",
                value: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000203"),
                column: "CategoryId",
                value: new Guid("00000000-0000-0000-0000-000000000004"));
        }
    }
}

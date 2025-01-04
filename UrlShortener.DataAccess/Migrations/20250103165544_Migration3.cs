using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UrlShortener.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Migration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0d9641a7-7a35-4406-89cc-7e7e6999e595"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3c854f7e-3932-4d59-b8ce-2a6f638f3095"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Urls",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0adceac9-c6c1-4fa1-af21-d6091e851e45"), null, "Admin", "ADMIN" },
                    { new Guid("d8040054-3a5c-4c73-ac45-77bdd6323abb"), null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0adceac9-c6c1-4fa1-af21-d6091e851e45"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d8040054-3a5c-4c73-ac45-77bdd6323abb"));

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Urls");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0d9641a7-7a35-4406-89cc-7e7e6999e595"), null, "User", "USER" },
                    { new Guid("3c854f7e-3932-4d59-b8ce-2a6f638f3095"), null, "Admin", "ADMIN" }
                });
        }
    }
}

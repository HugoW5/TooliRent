using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TooliRent.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7a7b5c20-1234-4c99-a9a1-8e1b51a7a111", null, "User", "USER" },
                    { "8d12c03f-8b7d-4b11-9d39-12ab3b45d3c1", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "5c77e6e4-4321-4e9e-8a19-f9b1c7c4fabc", 0, "STATIC-CONCURRENCY-STAMP", "admin@site.com", true, false, null, "ADMIN@SITE.COM", "ADMIN", "AQAAAAIAAYagAAAAEB2XN/evKfAfX3TSk6dGnJgUWve+fuEKmp8ShXUSlDMAZaTOwNgvW6/qcDXPE9Ftgw==", null, false, "STATIC-SECURITY-STAMP", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "8d12c03f-8b7d-4b11-9d39-12ab3b45d3c1", "5c77e6e4-4321-4e9e-8a19-f9b1c7c4fabc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7a7b5c20-1234-4c99-a9a1-8e1b51a7a111");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8d12c03f-8b7d-4b11-9d39-12ab3b45d3c1", "5c77e6e4-4321-4e9e-8a19-f9b1c7c4fabc" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8d12c03f-8b7d-4b11-9d39-12ab3b45d3c1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5c77e6e4-4321-4e9e-8a19-f9b1c7c4fabc");
        }
    }
}

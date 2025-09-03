using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TooliRent.Migrations
{
    /// <inheritdoc />
    public partial class member_role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7a7b5c20-1234-4c99-a9a1-8e1b51a7a111",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Member", "MEMBER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7a7b5c20-1234-4c99-a9a1-8e1b51a7a111",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "User", "USER" });
        }
    }
}

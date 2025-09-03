using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TooliRent.Migrations
{
    /// <inheritdoc />
    public partial class AdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5c77e6e4-4321-4e9e-8a19-f9b1c7c4fabc",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMQjUOb4dV0YwutubztJCQ0zAgbv35afKy5XJIInX8AQ9BBhLaV3QSucs/0LoWiHEw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5c77e6e4-4321-4e9e-8a19-f9b1c7c4fabc",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEB2XN/evKfAfX3TSk6dGnJgUWve+fuEKmp8ShXUSlDMAZaTOwNgvW6/qcDXPE9Ftgw==");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Discriminator", "FullName", "PasswordHash", "PhoneNumber", "Role", "Username" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "User", "Admin User", "$2a$11$Pj4bJPS.3wtrs/z3ko6krOSEpeBAHRLsh4kh3MRQ4uPIqS93mO6D6", "0000000000", 1, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}

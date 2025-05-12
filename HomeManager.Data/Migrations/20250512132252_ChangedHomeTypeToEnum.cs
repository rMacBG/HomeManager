using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedHomeTypeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "HomeType",
                table: "Homes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$SJ.3LN4.yQig1y5RyIeGK.Xp4FaPxnua6pYGl71ljVzr2ZtFEcdiC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HomeType",
                table: "Homes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$Pj4bJPS.3wtrs/z3ko6krOSEpeBAHRLsh4kh3MRQ4uPIqS93mO6D6");
        }
    }
}

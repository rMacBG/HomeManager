using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$tMOou7pTi0f8MkNYWO24TOB5Unk4ljMBwpdcrKoqIW.GuwBMKBQsq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$SJ.3LN4.yQig1y5RyIeGK.Xp4FaPxnua6pYGl71ljVzr2ZtFEcdiC");
        }
    }
}

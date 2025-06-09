using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class PicturesForHomesDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeImage_Homes_HomeId",
                table: "HomeImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HomeImage",
                table: "HomeImage");

            migrationBuilder.RenameTable(
                name: "HomeImage",
                newName: "HomeImages");

            migrationBuilder.RenameIndex(
                name: "IX_HomeImage_HomeId",
                table: "HomeImages",
                newName: "IX_HomeImages_HomeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HomeImages",
                table: "HomeImages",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$emWANEmvlyHz3Y5z73K/dut.kcPCzR2pgg6lj79tckd8bm4B4Qn62");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeImages_Homes_HomeId",
                table: "HomeImages",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeImages_Homes_HomeId",
                table: "HomeImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HomeImages",
                table: "HomeImages");

            migrationBuilder.RenameTable(
                name: "HomeImages",
                newName: "HomeImage");

            migrationBuilder.RenameIndex(
                name: "IX_HomeImages_HomeId",
                table: "HomeImage",
                newName: "IX_HomeImage_HomeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HomeImage",
                table: "HomeImage",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$EaIG1HvPboP0N9vNAfIAPemOgcdsZUPV2FtFWzPUVa7H.tgayXKti");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeImage_Homes_HomeId",
                table: "HomeImage",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

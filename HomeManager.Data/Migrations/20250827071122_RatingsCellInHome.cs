using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class RatingsCellInHome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$wyWnJH/HO16u62Cs.S1kTO4ZLiNm1QGq.6nTNzhurFmNIuLWqQB92");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_HomeId",
                table: "Ratings",
                column: "HomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Homes_HomeId",
                table: "Ratings",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Homes_HomeId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_HomeId",
                table: "Ratings");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$zIDELsn.9NUROW1dJo6RN..ex/5s7rn1eTxoDN.HX7wqv52PSlzii");
        }
    }
}

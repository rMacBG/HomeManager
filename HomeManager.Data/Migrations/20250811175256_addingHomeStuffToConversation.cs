using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingHomeStuffToConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HomeId",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$iSHpYNmbpKPml3wUHpCHguC5R11suP4pgEfMJuZWz/.l9eMvTCd76");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_HomeId",
                table: "Conversations",
                column: "HomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Homes_HomeId",
                table: "Conversations",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Homes_HomeId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_HomeId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "HomeId",
                table: "Conversations");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$b5R9xF4lOFbeTOCYv7NsHeSbyJlvbgKKMmhA7jqYs02AzIUXa3gt.");
        }
    }
}

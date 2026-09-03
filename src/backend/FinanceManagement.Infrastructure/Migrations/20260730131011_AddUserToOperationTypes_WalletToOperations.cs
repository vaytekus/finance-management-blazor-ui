using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToOperationTypes_WalletToOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationTypes_Name",
                table: "OperationTypes");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "OperationTypes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WalletId",
                table: "Operations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OperationTypes_UserId_Name",
                table: "OperationTypes",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_WalletId",
                table: "Operations",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Wallets_WalletId",
                table: "Operations",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationTypes_Users_UserId",
                table: "OperationTypes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Wallets_WalletId",
                table: "Operations");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationTypes_Users_UserId",
                table: "OperationTypes");

            migrationBuilder.DropIndex(
                name: "IX_OperationTypes_UserId_Name",
                table: "OperationTypes");

            migrationBuilder.DropIndex(
                name: "IX_Operations_WalletId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OperationTypes");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "Operations");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTypes_Name",
                table: "OperationTypes",
                column: "Name",
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsDeletedFromSoftDeletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId_Name",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Operations");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId_Name",
                table: "Wallets",
                columns: new[] { "UserId", "Name" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId_Name",
                table: "Wallets");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Wallets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Operations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId_Name",
                table: "Wallets",
                columns: new[] { "UserId", "Name" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}

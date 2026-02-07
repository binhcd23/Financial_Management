using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial_Management_Server.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionSoftDeleteAndNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "wallets",
                newName: "is_default");

            migrationBuilder.RenameColumn(
                name: "CardNumber",
                table: "wallets",
                newName: "card_number");

            migrationBuilder.RenameColumn(
                name: "CardHolderName",
                table: "wallets",
                newName: "card_holder_name");

            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "wallets",
                newName: "bank_id");

            migrationBuilder.AddColumn<bool>(
                name: "is_delete",
                table: "wallets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_delete",
                table: "transactions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_delete",
                table: "wallets");

            migrationBuilder.DropColumn(
                name: "is_delete",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "is_default",
                table: "wallets",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "card_number",
                table: "wallets",
                newName: "CardNumber");

            migrationBuilder.RenameColumn(
                name: "card_holder_name",
                table: "wallets",
                newName: "CardHolderName");

            migrationBuilder.RenameColumn(
                name: "bank_id",
                table: "wallets",
                newName: "BankId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial_Management_Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWalletWithBankInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "balance",
                table: "wallets",
                type: "decimal(15,2)",
                precision: 15,
                scale: 2,
                nullable: true,
                defaultValue: 0.00m,
                oldClrType: typeof(decimal),
                oldType: "decimal(15,2)",
                oldPrecision: 15,
                oldScale: 2,
                oldNullable: true,
                oldDefaultValueSql: "'0.00'");

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "wallets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardHolderName",
                table: "wallets",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "wallets",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "wallets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankId",
                table: "wallets");

            migrationBuilder.DropColumn(
                name: "CardHolderName",
                table: "wallets");

            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "wallets");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "wallets");

            migrationBuilder.AlterColumn<decimal>(
                name: "balance",
                table: "wallets",
                type: "decimal(15,2)",
                precision: 15,
                scale: 2,
                nullable: true,
                defaultValueSql: "'0.00'",
                oldClrType: typeof(decimal),
                oldType: "decimal(15,2)",
                oldPrecision: 15,
                oldScale: 2,
                oldNullable: true,
                oldDefaultValue: 0.00m);
        }
    }
}

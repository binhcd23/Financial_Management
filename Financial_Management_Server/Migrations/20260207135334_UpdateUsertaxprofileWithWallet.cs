using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial_Management_Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsertaxprofileWithWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "wallet_id",
                table: "usertaxprofile",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_usertaxprofile_wallet_id",
                table: "usertaxprofile",
                column: "wallet_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "usertaxprofile_ibfk_2",
                table: "usertaxprofile",
                column: "wallet_id",
                principalTable: "wallets",
                principalColumn: "wallet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "usertaxprofile_ibfk_2",
                table: "usertaxprofile");

            migrationBuilder.DropIndex(
                name: "IX_usertaxprofile_wallet_id",
                table: "usertaxprofile");

            migrationBuilder.DropColumn(
                name: "wallet_id",
                table: "usertaxprofile");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial_Management_Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaxAndGoalNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "taxbrackets");

            migrationBuilder.DropTable(
                name: "taxconstants");

            migrationBuilder.DropColumn(
                name: "dependent_count",
                table: "usertaxprofile");

            migrationBuilder.DropColumn(
                name: "is_resident",
                table: "usertaxprofile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "dependent_count",
                table: "usertaxprofile",
                type: "int",
                nullable: true,
                defaultValueSql: "'0'");

            migrationBuilder.AddColumn<bool>(
                name: "is_resident",
                table: "usertaxprofile",
                type: "tinyint(1)",
                nullable: true,
                defaultValueSql: "'1'");

            migrationBuilder.CreateTable(
                name: "taxbrackets",
                columns: table => new
                {
                    bracket_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    deduction_amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    tax_rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    threshold_from = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    threshold_to = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.bracket_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "taxconstants",
                columns: table => new
                {
                    constant_key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    constant_value = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.constant_key);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }
    }
}

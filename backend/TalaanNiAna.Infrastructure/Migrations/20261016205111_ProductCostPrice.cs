using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalaanNiAna.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductCostPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "TransactionItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "Products");
        }
    }
}

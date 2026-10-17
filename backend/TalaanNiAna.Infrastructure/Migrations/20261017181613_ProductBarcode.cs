using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalaanNiAna.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductBarcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId_Barcode",
                table: "Products",
                columns: new[] { "StoreId", "Barcode" },
                unique: true,
                filter: "[Barcode] IS NOT NULL AND [IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_StoreId_Barcode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Products");
        }
    }
}

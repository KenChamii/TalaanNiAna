using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalaanNiAna.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreSettings");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_IdempotencyKey",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionDate",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Products_Category",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StoreName",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "TransactionItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "CreditPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LowStockAlertLimit = table.Column<int>(type: "int", nullable: false),
                    SmsAlertTemplate = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_StoreId_IdempotencyKey",
                table: "Transactions",
                columns: new[] { "StoreId", "IdempotencyKey" },
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_StoreId_TransactionDate",
                table: "Transactions",
                columns: new[] { "StoreId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItems_StoreId",
                table: "TransactionItems",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId_Category",
                table: "Products",
                columns: new[] { "StoreId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_StoreId",
                table: "Customers",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditPayments_StoreId_PaidAt",
                table: "CreditPayments",
                columns: new[] { "StoreId", "PaidAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StoreId",
                table: "AspNetUsers",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Stores_StoreId",
                table: "AspNetUsers",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Stores_StoreId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_StoreId_IdempotencyKey",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_StoreId_TransactionDate",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_TransactionItems_StoreId",
                table: "TransactionItems");

            migrationBuilder.DropIndex(
                name: "IX_Products_StoreId_Category",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Customers_StoreId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_CreditPayments_StoreId_PaidAt",
                table: "CreditPayments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StoreId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "CreditPayments");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "StoreName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StoreSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LowStockAlertLimit = table.Column<int>(type: "int", nullable: false),
                    SmsAlertTemplate = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "StoreSettings",
                columns: new[] { "Id", "LowStockAlertLimit", "SmsAlertTemplate", "StoreName", "TimeZoneId" },
                values: new object[] { 1, 10, "Hi Suki, paalala lang...", "Nena's Sari-Sari Store", "Asia/Manila" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IdempotencyKey",
                table: "Transactions",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDate",
                table: "Transactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");
        }
    }
}

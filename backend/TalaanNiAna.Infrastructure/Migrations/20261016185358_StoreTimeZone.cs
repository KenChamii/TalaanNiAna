using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalaanNiAna.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StoreTimeZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StoreName",
                table: "StoreSettings",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SmsAlertTemplate",
                table: "StoreSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "StoreSettings",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "StoreSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "TimeZoneId",
                value: "Asia/Manila");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "StoreSettings");

            migrationBuilder.AlterColumn<string>(
                name: "StoreName",
                table: "StoreSettings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "SmsAlertTemplate",
                table: "StoreSettings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}

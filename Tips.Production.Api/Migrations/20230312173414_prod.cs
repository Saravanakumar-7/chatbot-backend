using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tips.Production.Api.Migrations
{
    /// <inheritdoc />
    public partial class prod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "WipQty",
                table: "ShopOrders",
                type: "decimal(13,3)",
                precision: 13,
                scale: 3,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,3)",
                oldPrecision: 13,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalSOReleaseQty",
                table: "ShopOrders",
                type: "decimal(13,3)",
                precision: 13,
                scale: 3,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,3)",
                oldPrecision: 13,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ScrapQty",
                table: "ShopOrders",
                type: "decimal(13,3)",
                precision: 13,
                scale: 3,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,3)",
                oldPrecision: 13,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OqcQty",
                table: "ShopOrders",
                type: "decimal(13,3)",
                precision: 13,
                scale: 3,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,3)",
                oldPrecision: 13,
                oldScale: 3,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "WipQty",
                table: "ShopOrders",
                type: "decimal(13,3)",
                precision: 13,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,3)",
                oldPrecision: 13,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalSOReleaseQty",
                table: "ShopOrders",
                type: "decimal(13,3)",
                precision: 13,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,3)",
                oldPrecision: 13,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "ScrapQty",
                table: "ShopOrders",
                type: "decimal(13,3)",
                precision: 13,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,3)",
                oldPrecision: 13,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "OqcQty",
                table: "ShopOrders",
                type: "decimal(13,3)",
                precision: 13,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,3)",
                oldPrecision: 13,
                oldScale: 3);
        }
    }
}

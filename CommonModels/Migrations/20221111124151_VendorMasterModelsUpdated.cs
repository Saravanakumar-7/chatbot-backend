using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class VendorMasterModelsUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_VendorMasters_VendorMasterId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_VendorMasters_VendorMasterId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorBankings_VendorMasters_VendorMasterId",
                table: "VendorBankings");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "VendorBankings");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Addresses");

            migrationBuilder.AlterColumn<int>(
                name: "VendorMasterId",
                table: "VendorBankings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VendorMasterId",
                table: "Contact",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VendorMasterId",
                table: "Addresses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_VendorMasters_VendorMasterId",
                table: "Addresses",
                column: "VendorMasterId",
                principalTable: "VendorMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_VendorMasters_VendorMasterId",
                table: "Contact",
                column: "VendorMasterId",
                principalTable: "VendorMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorBankings_VendorMasters_VendorMasterId",
                table: "VendorBankings",
                column: "VendorMasterId",
                principalTable: "VendorMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_VendorMasters_VendorMasterId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_VendorMasters_VendorMasterId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorBankings_VendorMasters_VendorMasterId",
                table: "VendorBankings");

            migrationBuilder.AlterColumn<int>(
                name: "VendorMasterId",
                table: "VendorBankings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "VendorBankings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "VendorMasterId",
                table: "Contact",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "Contact",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "VendorMasterId",
                table: "Addresses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "Addresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_VendorMasters_VendorMasterId",
                table: "Addresses",
                column: "VendorMasterId",
                principalTable: "VendorMasters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_VendorMasters_VendorMasterId",
                table: "Contact",
                column: "VendorMasterId",
                principalTable: "VendorMasters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorBankings_VendorMasters_VendorMasterId",
                table: "VendorBankings",
                column: "VendorMasterId",
                principalTable: "VendorMasters",
                principalColumn: "Id");
        }
    }
}

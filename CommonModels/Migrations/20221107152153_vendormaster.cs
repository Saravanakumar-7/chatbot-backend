using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class vendormaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Active",
                table: "Contact",
                newName: "IsActive");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "VendorMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "VendorMasterBankings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VendorMasterId",
                table: "VendorMasterBankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "Contact",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VendorMasterId",
                table: "Contact",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "Addresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VendorMasterId",
                table: "Addresses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorMasterBankings_VendorMasterId",
                table: "VendorMasterBankings",
                column: "VendorMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_VendorMasterId",
                table: "Contact",
                column: "VendorMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_VendorMasterId",
                table: "Addresses",
                column: "VendorMasterId");

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
                name: "FK_VendorMasterBankings_VendorMasters_VendorMasterId",
                table: "VendorMasterBankings",
                column: "VendorMasterId",
                principalTable: "VendorMasters",
                principalColumn: "Id");
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
                name: "FK_VendorMasterBankings_VendorMasters_VendorMasterId",
                table: "VendorMasterBankings");

            migrationBuilder.DropIndex(
                name: "IX_VendorMasterBankings_VendorMasterId",
                table: "VendorMasterBankings");

            migrationBuilder.DropIndex(
                name: "IX_Contact_VendorMasterId",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_VendorMasterId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "VendorMasters");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "VendorMasterBankings");

            migrationBuilder.DropColumn(
                name: "VendorMasterId",
                table: "VendorMasterBankings");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "VendorMasterId",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "VendorMasterId",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Contact",
                newName: "Active");
        }
    }
}

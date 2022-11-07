using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class _7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GSTNNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PANNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SameasAddress = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Salutation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LandLine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeToCall = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Primary = table.Column<bool>(type: "bit", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternameMobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorMasterBankings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IFSCCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SwiftCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IBANCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Primary = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorMasterBankings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VendorAliasName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PinZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneralEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GooglePinLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOfCompany = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExportUnitType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneralMSME = table.Column<bool>(type: "bit", nullable: false),
                    RelatedVendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedVendorAlias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NatureOfRelationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TermAdvance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncoTerm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreferredFreightForwader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InCoporation = table.Column<bool>(type: "bit", nullable: false),
                    TIN = table.Column<bool>(type: "bit", nullable: false),
                    GST = table.Column<bool>(type: "bit", nullable: false),
                    IEC = table.Column<bool>(type: "bit", nullable: false),
                    PAN = table.Column<bool>(type: "bit", nullable: false),
                    UdhyamCertificate = table.Column<bool>(type: "bit", nullable: false),
                    MSME = table.Column<bool>(type: "bit", nullable: false),
                    ISO = table.Column<bool>(type: "bit", nullable: false),
                    AS = table.Column<bool>(type: "bit", nullable: false),
                    Medical = table.Column<bool>(type: "bit", nullable: false),
                    NADCAP = table.Column<bool>(type: "bit", nullable: false),
                    TurnOver3years = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DNBHooversNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ICRA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadCount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UOM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FloorSpace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sqft = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Machine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToolsandEquip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ERPvalue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ERP = table.Column<bool>(type: "bit", nullable: false),
                    ESDSetup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HazmatSetup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSPvalue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSP = table.Column<bool>(type: "bit", nullable: false),
                    ScopeOfSupply = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankOfApproval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InventoryItem = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Upload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReAudit = table.Column<bool>(type: "bit", nullable: false),
                    AuditFrequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorMasters", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "VendorMasterBankings");

            migrationBuilder.DropTable(
                name: "VendorMasters");
        }
    }
}

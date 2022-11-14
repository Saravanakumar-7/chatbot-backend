using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class tablef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditFrequencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditFrequencyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditFrequencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostCenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostCenterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostingMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostingMethodName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostingMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExportUnitTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExportUnitTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportUnitTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemMasters",
                columns: table => new
                {
                    ItemMasterId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Uom = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Commodity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Hsn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaterialGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchaseGroup = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerPartReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPRRequired = table.Column<bool>(type: "bit", nullable: false),
                    PoMaterialType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OpenGrin = table.Column<bool>(type: "bit", nullable: false),
                    IsCustomerSuppliedItem = table.Column<bool>(type: "bit", nullable: false),
                    DrawingNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocRet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCocRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsRohsItem = table.Column<bool>(type: "bit", nullable: false),
                    IsShelfLife = table.Column<bool>(type: "bit", nullable: false),
                    IsReachItem = table.Column<bool>(type: "bit", nullable: false),
                    NetWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetUom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossUom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VolumeUom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FootPrint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Min = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Max = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Leadtime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reorder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoBin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kanban = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEsd = table.Column<bool>(type: "bit", nullable: false),
                    IsFifo = table.Column<bool>(type: "bit", nullable: false),
                    IsLifo = table.Column<bool>(type: "bit", nullable: false),
                    IsCycleCount = table.Column<bool>(type: "bit", nullable: false),
                    IsHazardousMaterial = table.Column<bool>(type: "bit", nullable: false),
                    Expiry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionInterval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingInstruction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsIQCRequired = table.Column<bool>(type: "bit", nullable: false),
                    GrProcessing = table.Column<int>(type: "int", nullable: false),
                    BatchSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostCenter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StdCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostingMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valuation = table.Column<bool>(type: "bit", nullable: false),
                    Depreciation = table.Column<bool>(type: "bit", nullable: false),
                    Pfo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMasters", x => x.ItemMasterId);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NatureOfRelationships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NatureOfRelationshipName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NatureOfRelationships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackingInstructions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackingInstructionsName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingInstructions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentTerms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseGroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salutations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalutationsName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salutations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeOfCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeOfCompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeOfCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemmasterAlternate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerPartNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    ItemMasterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemmasterAlternate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemmasterAlternate_ItemMasters_ItemMasterId",
                        column: x => x.ItemMasterId,
                        principalTable: "ItemMasters",
                        principalColumn: "ItemMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemMasterApprovedVendor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShareOfBusiness = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemMasterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMasterApprovedVendor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemMasterApprovedVendor_ItemMasters_ItemMasterId",
                        column: x => x.ItemMasterId,
                        principalTable: "ItemMasters",
                        principalColumn: "ItemMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemMasterFileUpload",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemMasterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMasterFileUpload", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemMasterFileUpload_ItemMasters_ItemMasterId",
                        column: x => x.ItemMasterId,
                        principalTable: "ItemMasters",
                        principalColumn: "ItemMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemMasterRouting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessStep = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Process = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoutingDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineHours = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaborHours = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRoutingActive = table.Column<bool>(type: "bit", nullable: false),
                    ItemMasterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMasterRouting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemMasterRouting_ItemMasters_ItemMasterId",
                        column: x => x.ItemMasterId,
                        principalTable: "ItemMasters",
                        principalColumn: "ItemMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemMasterWarehouse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WareHouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ItemMasterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMasterWarehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemMasterWarehouse_ItemMasters_ItemMasterId",
                        column: x => x.ItemMasterId,
                        principalTable: "ItemMasters",
                        principalColumn: "ItemMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemmasterAlternate_ItemMasterId",
                table: "ItemmasterAlternate",
                column: "ItemMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasterApprovedVendor_ItemMasterId",
                table: "ItemMasterApprovedVendor",
                column: "ItemMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasterFileUpload_ItemMasterId",
                table: "ItemMasterFileUpload",
                column: "ItemMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasterRouting_ItemMasterId",
                table: "ItemMasterRouting",
                column: "ItemMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasterWarehouse_ItemMasterId",
                table: "ItemMasterWarehouse",
                column: "ItemMasterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditFrequencies");

            migrationBuilder.DropTable(
                name: "CostCenters");

            migrationBuilder.DropTable(
                name: "CostingMethods");

            migrationBuilder.DropTable(
                name: "ExportUnitTypes");

            migrationBuilder.DropTable(
                name: "ItemmasterAlternate");

            migrationBuilder.DropTable(
                name: "ItemMasterApprovedVendor");

            migrationBuilder.DropTable(
                name: "ItemMasterFileUpload");

            migrationBuilder.DropTable(
                name: "ItemMasterRouting");

            migrationBuilder.DropTable(
                name: "ItemMasterWarehouse");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "NatureOfRelationships");

            migrationBuilder.DropTable(
                name: "PackingInstructions");

            migrationBuilder.DropTable(
                name: "PaymentTerms");

            migrationBuilder.DropTable(
                name: "PurchaseGroups");

            migrationBuilder.DropTable(
                name: "Salutations");

            migrationBuilder.DropTable(
                name: "TypeOfCompanies");

            migrationBuilder.DropTable(
                name: "ItemMasters");
        }
    }
}

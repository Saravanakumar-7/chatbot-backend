using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tips.Purchase.Api.Migrations
{
    /// <inheritdoc />
    public partial class rfqfinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PODate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: true),
                    ProcurementType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuotationReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuotationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VendorAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetentionPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialTermsAndConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    POApprovalI = table.Column<bool>(type: "bit", nullable: false),
                    POApprovedIBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    POApprovedIDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POApprovalII = table.Column<bool>(type: "bit", nullable: false),
                    POApprovedIIBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    POApprovedIIDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsShortClosed = table.Column<bool>(type: "bit", nullable: false),
                    ShortClosedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortClosedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequisitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevisionNumber = table.Column<int>(type: "int", nullable: true),
                    ProcurementType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetentionPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialTermsConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsShortClosed = table.Column<bool>(type: "bit", nullable: false),
                    ShortClosedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortClosedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrApprovalI = table.Column<bool>(type: "bit", nullable: false),
                    PrApprovedIBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrApprovedIDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrApprovalII = table.Column<bool>(type: "bit", nullable: false),
                    PrApprovedIIBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrApprovedIIDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequisitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MftrItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UOM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BalanceQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PartType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialInstruction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTechnicalDocsRequired = table.Column<bool>(type: "bit", nullable: false),
                    PoPartsStatus = table.Column<bool>(type: "bit", nullable: false),
                    SGST = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    CGST = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    IGST = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    UTGST = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    TotalWithTax = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoItems_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentUploads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentFrom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: true),
                    PurchaseRequisitionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentUploads_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentUploads_PurchaseRequisitions_PurchaseRequisitionId",
                        column: x => x.PurchaseRequisitionId,
                        principalTable: "PurchaseRequisitions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PRDocumentUploads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentFrom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PurchaseRequisitionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRDocumentUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PRDocumentUploads_PurchaseRequisitions_PurchaseRequisitionId",
                        column: x => x.PurchaseRequisitionId,
                        principalTable: "PurchaseRequisitions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PrItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MftrItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UOM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    SpecialInstruction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PurchaseRequistionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrItems_PurchaseRequisitions_PurchaseRequistionId",
                        column: x => x.PurchaseRequistionId,
                        principalTable: "PurchaseRequisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PoAddDeliverySchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PODeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PODeliveryQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    POItemDetailId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoAddDeliverySchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoAddDeliverySchedules_PoItems_POItemDetailId",
                        column: x => x.POItemDetailId,
                        principalTable: "PoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PoAddProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    POItemDetailId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoAddProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoAddProjects_PoItems_POItemDetailId",
                        column: x => x.POItemDetailId,
                        principalTable: "PoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrAddDeliverySchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrDeliveryQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    PrItemDetailId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrAddDeliverySchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrAddDeliverySchedules_PrItems_PrItemDetailId",
                        column: x => x.PrItemDetailId,
                        principalTable: "PrItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrAddProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    PrItemDetailId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrAddProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrAddProjects_PrItems_PrItemDetailId",
                        column: x => x.PrItemDetailId,
                        principalTable: "PrItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_PurchaseOrderId",
                table: "DocumentUploads",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_PurchaseRequisitionId",
                table: "DocumentUploads",
                column: "PurchaseRequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PoAddDeliverySchedules_POItemDetailId",
                table: "PoAddDeliverySchedules",
                column: "POItemDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PoAddProjects_POItemDetailId",
                table: "PoAddProjects",
                column: "POItemDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PoItems_PurchaseOrderId",
                table: "PoItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PrAddDeliverySchedules_PrItemDetailId",
                table: "PrAddDeliverySchedules",
                column: "PrItemDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PrAddProjects_PrItemDetailId",
                table: "PrAddProjects",
                column: "PrItemDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PRDocumentUploads_PurchaseRequisitionId",
                table: "PRDocumentUploads",
                column: "PurchaseRequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrItems_PurchaseRequistionId",
                table: "PrItems",
                column: "PurchaseRequistionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentUploads");

            migrationBuilder.DropTable(
                name: "PoAddDeliverySchedules");

            migrationBuilder.DropTable(
                name: "PoAddProjects");

            migrationBuilder.DropTable(
                name: "PrAddDeliverySchedules");

            migrationBuilder.DropTable(
                name: "PrAddProjects");

            migrationBuilder.DropTable(
                name: "PRDocumentUploads");

            migrationBuilder.DropTable(
                name: "PoItems");

            migrationBuilder.DropTable(
                name: "PrItems");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "PurchaseRequisitions");
        }
    }
}

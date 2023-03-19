using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tips.Production.Api.Migrations
{
    /// <inheritdoc />
    public partial class production : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FGShopOrderMaterialIssues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopOrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FGPartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopOrderQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    ShopOrderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FGShopOrderMaterialIssues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialIssue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopOrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    ShopOrderQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaterialIssuedStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialIssue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialReturnNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MRNNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopOrderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReturnNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "oQCs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    ShopOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopOrderQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    PendingQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    AcceptedQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    RejectedQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oQCs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SAShopOrderMaterialIssues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SAShopOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAShopOrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FGPartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAShopOrderQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    ShopOrderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAShopOrderMaterialIssues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SAShopOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SAShopOrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProjectType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProjectNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FGItemNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SAItemNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SalesOrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesOrderQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    SAShopOrderReleaseQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    SAShopOrderCloseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SalesOrderPONumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WipQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    OqcQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    ScrapQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsShortClosed = table.Column<bool>(type: "bit", nullable: false),
                    ShortClosedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShorClosedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaterialIssueStatus = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAShopOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrderConfirmations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    ShopOrderReleaseQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    WipConfirmedQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    IsOQCDone = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOrderConfirmations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    TotalSOReleaseQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    SOCloseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BomRevisionNo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CanCreateQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    WipQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    OqcQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    ScrapQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    FGDoneStatus = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ShopOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FGShopOrderMaterialIssueGenerals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UOM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    RequiredQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    AvailableQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    AlreadyIssuedQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    IssueQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FGShopOrderMaterialIssueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FGShopOrderMaterialIssueGenerals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FGShopOrderMaterialIssueGenerals_FGShopOrderMaterialIssues_FGShopOrderMaterialIssueId",
                        column: x => x.FGShopOrderMaterialIssueId,
                        principalTable: "FGShopOrderMaterialIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialIssueItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PartType = table.Column<int>(type: "int", nullable: false),
                    UOM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    AvailableQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    IssuedQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", precision: 13, scale: 3, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaterialIssuedStatus = table.Column<int>(type: "int", nullable: false),
                    MaterialIssueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialIssueItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialIssueItem_MaterialIssue_MaterialIssueId",
                        column: x => x.MaterialIssueId,
                        principalTable: "MaterialIssue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialReturnNoteItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MftrPartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Warehouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaterialReturnNoteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReturnNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialReturnNoteItems_MaterialReturnNotes_MaterialReturnNoteId",
                        column: x => x.MaterialReturnNoteId,
                        principalTable: "MaterialReturnNotes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SAShopOrderMaterialIssueGenerals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UOM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    RequiredQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    AvailableQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    AlreadyIssuedQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    IssueQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SAShopOrderMaterialIssueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAShopOrderMaterialIssueGenerals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SAShopOrderMaterialIssueGenerals_SAShopOrderMaterialIssues_SAShopOrderMaterialIssueId",
                        column: x => x.SAShopOrderMaterialIssueId,
                        principalTable: "SAShopOrderMaterialIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FGItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenSalesOrderQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    ReleaseQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    RequiredQty = table.Column<decimal>(type: "decimal(13,3)", precision: 13, scale: 3, nullable: true),
                    ShopOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopOrderItems_ShopOrders_ShopOrderId",
                        column: x => x.ShopOrderId,
                        principalTable: "ShopOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FGShopOrderMaterialIssueGenerals_FGShopOrderMaterialIssueId",
                table: "FGShopOrderMaterialIssueGenerals",
                column: "FGShopOrderMaterialIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssueItem_MaterialIssueId",
                table: "MaterialIssueItem",
                column: "MaterialIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReturnNoteItems_MaterialReturnNoteId",
                table: "MaterialReturnNoteItems",
                column: "MaterialReturnNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_SAShopOrderMaterialIssueGenerals_SAShopOrderMaterialIssueId",
                table: "SAShopOrderMaterialIssueGenerals",
                column: "SAShopOrderMaterialIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderItems_ShopOrderId",
                table: "ShopOrderItems",
                column: "ShopOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FGShopOrderMaterialIssueGenerals");

            migrationBuilder.DropTable(
                name: "MaterialIssueItem");

            migrationBuilder.DropTable(
                name: "MaterialReturnNoteItems");

            migrationBuilder.DropTable(
                name: "oQCs");

            migrationBuilder.DropTable(
                name: "SAShopOrderMaterialIssueGenerals");

            migrationBuilder.DropTable(
                name: "SAShopOrders");

            migrationBuilder.DropTable(
                name: "ShopOrderConfirmations");

            migrationBuilder.DropTable(
                name: "ShopOrderItems");

            migrationBuilder.DropTable(
                name: "FGShopOrderMaterialIssues");

            migrationBuilder.DropTable(
                name: "MaterialIssue");

            migrationBuilder.DropTable(
                name: "MaterialReturnNotes");

            migrationBuilder.DropTable(
                name: "SAShopOrderMaterialIssues");

            migrationBuilder.DropTable(
                name: "ShopOrders");
        }
    }
}

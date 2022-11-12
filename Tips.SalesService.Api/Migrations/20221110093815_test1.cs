using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tips.SalesService.Api.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "RevNumber",
                table: "rfqs",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RfqNumber",
                table: "rfqs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "rfqCustomFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduleQuantity = table.Column<int>(type: "int", nullable: true),
                    SelectCustomGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomFieldSchedule = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SelectCustomGroups = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxLengthUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RfqNumber = table.Column<int>(type: "int", nullable: false),
                    RfqId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfqCustomFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rfqCustomFields_rfqs_RfqId",
                        column: x => x.RfqId,
                        principalTable: "rfqs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "rfqCustomGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomGroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RfqNumber = table.Column<int>(type: "int", nullable: false),
                    RfqId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfqCustomGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rfqCustomGroups_rfqs_RfqId",
                        column: x => x.RfqId,
                        principalTable: "rfqs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "rfqEngineerings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemMaster = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnggCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnggNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RfqNumber = table.Column<int>(type: "int", nullable: false),
                    RfqId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfqEngineerings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rfqEngineerings_rfqs_RfqId",
                        column: x => x.RfqId,
                        principalTable: "rfqs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_rfqCustomFields_RfqId",
                table: "rfqCustomFields",
                column: "RfqId");

            migrationBuilder.CreateIndex(
                name: "IX_rfqCustomGroups_RfqId",
                table: "rfqCustomGroups",
                column: "RfqId");

            migrationBuilder.CreateIndex(
                name: "IX_rfqEngineerings_RfqId",
                table: "rfqEngineerings",
                column: "RfqId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rfqCustomFields");

            migrationBuilder.DropTable(
                name: "rfqCustomGroups");

            migrationBuilder.DropTable(
                name: "rfqEngineerings");

            migrationBuilder.DropColumn(
                name: "RevNumber",
                table: "rfqs");

            migrationBuilder.DropColumn(
                name: "RfqNumber",
                table: "rfqs");
        }
    }
}

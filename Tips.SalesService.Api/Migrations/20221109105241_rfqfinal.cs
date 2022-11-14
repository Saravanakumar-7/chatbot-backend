using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tips.SalesService.Api.Migrations
{
    /// <inheritdoc />
    public partial class rfqfinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rfqNumbers",
                columns: table => new
                {
                    RfqNO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfqNumbers", x => x.RfqNO);
                });

            migrationBuilder.CreateTable(
                name: "rfqs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RfqNumbers = table.Column<int>(type: "int", nullable: false),
                    CustomerRfqNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuoteExpectDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfqs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rfqNumbers");

            migrationBuilder.DropTable(
                name: "rfqs");
        }
    }
}

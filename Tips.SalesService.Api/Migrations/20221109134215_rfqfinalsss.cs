using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tips.SalesService.Api.Migrations
{
    /// <inheritdoc />
    public partial class rfqfinalsss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestReceiveDate",
                table: "rfqs",
                newName: "RequestReceivedate");

            migrationBuilder.RenameColumn(
                name: "QuoteExpectDate",
                table: "rfqs",
                newName: "QuoteExpectdate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestReceivedate",
                table: "rfqs",
                newName: "RequestReceiveDate");

            migrationBuilder.RenameColumn(
                name: "QuoteExpectdate",
                table: "rfqs",
                newName: "QuoteExpectDate");
        }
    }
}

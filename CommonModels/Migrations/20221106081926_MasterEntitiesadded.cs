using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class MasterEntitiesadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ProcurementTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ProcurementTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ProcurementTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "ProcurementTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "MaterialTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "MaterialTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "MaterialTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "MaterialTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "LeadTimes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "LeadTimes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "LeadTimes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "LeadTimes",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProcurementTypes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ProcurementTypes");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ProcurementTypes");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "ProcurementTypes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MaterialTypes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "MaterialTypes");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "MaterialTypes");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "MaterialTypes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LeadTimes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "LeadTimes");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "LeadTimes");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "LeadTimes");
        }
    }
}

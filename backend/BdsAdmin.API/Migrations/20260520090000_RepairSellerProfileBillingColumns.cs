using BdsAdmin.API.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BdsAdmin.API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260520090000_RepairSellerProfileBillingColumns")]
    public partial class RepairSellerProfileBillingColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "SellerProfiles"
                    ADD COLUMN IF NOT EXISTS "BudgetUnitCode" character varying(50),
                    ADD COLUMN IF NOT EXISTS "CitizenId" character varying(20),
                    ADD COLUMN IF NOT EXISTS "InvoiceAddress" character varying(300),
                    ADD COLUMN IF NOT EXISTS "InvoiceBuyerName" character varying(100),
                    ADD COLUMN IF NOT EXISTS "InvoiceCompanyName" character varying(150),
                    ADD COLUMN IF NOT EXISTS "InvoiceEmail" character varying(150),
                    ADD COLUMN IF NOT EXISTS "PassportNumber" character varying(30),
                    ADD COLUMN IF NOT EXISTS "TaxCode" character varying(50);
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetUnitCode",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "CitizenId",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "InvoiceAddress",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "InvoiceBuyerName",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "InvoiceCompanyName",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "InvoiceEmail",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "SellerProfiles");
        }
    }
}

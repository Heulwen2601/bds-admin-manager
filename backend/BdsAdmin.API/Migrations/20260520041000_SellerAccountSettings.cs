using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BdsAdmin.API.Migrations
{
    public partial class SellerAccountSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BudgetUnitCode",
                table: "SellerProfiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CitizenId",
                table: "SellerProfiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceAddress",
                table: "SellerProfiles",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceBuyerName",
                table: "SellerProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCompanyName",
                table: "SellerProfiles",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceEmail",
                table: "SellerProfiles",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "SellerProfiles",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
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

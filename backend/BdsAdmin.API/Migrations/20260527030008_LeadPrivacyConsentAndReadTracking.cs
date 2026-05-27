using Microsoft.EntityFrameworkCore.Migrations;

using System;

#nullable disable

namespace BdsAdmin.API.Migrations
{
    /// <inheritdoc />
    public partial class LeadPrivacyConsentAndReadTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Leads",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Leads",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerDataPolicyAcceptedAt",
                table: "SellerProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAcceptedCustomerDataPolicy",
                table: "SellerProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "CustomerDataPolicyAcceptedAt",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "HasAcceptedCustomerDataPolicy",
                table: "SellerProfiles");
        }
    }
}

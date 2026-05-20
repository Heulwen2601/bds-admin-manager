using BdsAdmin.API.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BdsAdmin.API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260520120000_AddSellerProfileAdditionalPhone")]
    public partial class AddSellerProfileAdditionalPhone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "SellerProfiles"
                    ADD COLUMN IF NOT EXISTS "AdditionalPhone" character varying(20);
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalPhone",
                table: "SellerProfiles");
        }
    }
}

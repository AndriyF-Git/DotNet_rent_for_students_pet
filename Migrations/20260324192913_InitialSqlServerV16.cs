using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rent_for_students.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServerV16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HousingListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    City = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    District = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    PricePerMonth = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    RoomType = table.Column<int>(type: "int", nullable: false),
                    AreaSqm = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousingListings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RentalApplicationProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalApplicationProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RentalApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalApplications_HousingListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "HousingListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HousingListings_City",
                table: "HousingListings",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_HousingListings_IsActive",
                table: "HousingListings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HousingListings_PricePerMonth",
                table: "HousingListings",
                column: "PricePerMonth");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplicationProfiles_ProfileName",
                table: "RentalApplicationProfiles",
                column: "ProfileName");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplicationProfiles_UpdatedAtUtc",
                table: "RentalApplicationProfiles",
                column: "UpdatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_CreatedAtUtc",
                table: "RentalApplications",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_ListingId",
                table: "RentalApplications",
                column: "ListingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RentalApplicationProfiles");

            migrationBuilder.DropTable(
                name: "RentalApplications");

            migrationBuilder.DropTable(
                name: "HousingListings");
        }
    }
}

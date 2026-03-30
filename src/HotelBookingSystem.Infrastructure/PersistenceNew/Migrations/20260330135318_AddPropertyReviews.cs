using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingSystem.Infrastructure.PersistenceNew.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyReviews",
                schema: "hotel_booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyReviews_AppUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "hotel_booking",
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyReviews_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalSchema: "hotel_booking",
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyReviews_PropertyId",
                schema: "hotel_booking",
                table: "PropertyReviews",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyReviews_UserId",
                schema: "hotel_booking",
                table: "PropertyReviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyReviews",
                schema: "hotel_booking");
        }
    }
}

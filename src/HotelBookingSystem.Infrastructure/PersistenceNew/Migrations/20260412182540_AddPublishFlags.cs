using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingSystem.Infrastructure.PersistenceNew.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                schema: "hotel_booking",
                table: "RoomTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                schema: "hotel_booking",
                table: "Rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                schema: "hotel_booking",
                table: "Properties",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                schema: "hotel_booking",
                table: "Beds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                schema: "hotel_booking",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                schema: "hotel_booking",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                schema: "hotel_booking",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                schema: "hotel_booking",
                table: "Beds");
        }
    }
}

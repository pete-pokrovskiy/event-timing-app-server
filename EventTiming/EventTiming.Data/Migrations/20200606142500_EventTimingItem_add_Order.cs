using Microsoft.EntityFrameworkCore.Migrations;

namespace EventTiming.Data.Migrations
{
    public partial class EventTimingItem_add_Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "EventTimingItem",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "EventTimingItem");
        }
    }
}

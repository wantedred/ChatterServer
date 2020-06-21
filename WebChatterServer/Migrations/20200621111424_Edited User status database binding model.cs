using Microsoft.EntityFrameworkCore.Migrations;

namespace WebChatterServer.Migrations
{
    public partial class EditedUserstatusdatabasebindingmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "UserStatuses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "UserStatuses");
        }
    }
}

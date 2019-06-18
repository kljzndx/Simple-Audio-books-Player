using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleAudioBooksPlayer.DAL.Migrations
{
    public partial class AddColumn_CurrentTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentTitle",
                table: "PlaybackRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentTitle",
                table: "PlaybackRecords");
        }
    }
}

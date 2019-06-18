using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleAudioBooksPlayer.DAL.Migrations
{
    public partial class AddTable_PlaybackRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "TrackNumber",
                table: "MusicFiles",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateTable(
                name: "PlaybackRecords",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false),
                    TrackId = table.Column<uint>(nullable: false),
                    SortMethod = table.Column<string>(nullable: true),
                    IsReverse = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaybackRecords", x => x.GroupId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaybackRecords");

            migrationBuilder.DropColumn(
                name: "TrackNumber",
                table: "MusicFiles");
        }
    }
}

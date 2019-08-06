using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleAudioBooksPlayer.DAL.Migrations
{
    public partial class AddTable_SubtitleIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubtitleFileIndexes",
                columns: table => new
                {
                    Index = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MusicFilePath = table.Column<string>(nullable: true),
                    SubtitleFilePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubtitleFileIndexes", x => x.Index);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubtitleFileIndexes");
        }
    }
}

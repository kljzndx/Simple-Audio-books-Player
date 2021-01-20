using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleAudioBooksPlayer.DAL.Migrations
{
    public partial class Drop_FileModel_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicFiles");

            migrationBuilder.DropTable(
                name: "SubtitleFiles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MusicFiles",
                columns: table => new
                {
                    FilePath = table.Column<string>(nullable: false),
                    DbVersion = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    ParentFolderName = table.Column<string>(nullable: true),
                    ParentFolderPath = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    TrackNumber = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicFiles", x => x.FilePath);
                });

            migrationBuilder.CreateTable(
                name: "SubtitleFiles",
                columns: table => new
                {
                    FilePath = table.Column<string>(nullable: false),
                    DbVersion = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    ParentFolderName = table.Column<string>(nullable: true),
                    ParentFolderPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubtitleFiles", x => x.FilePath);
                });
        }
    }
}

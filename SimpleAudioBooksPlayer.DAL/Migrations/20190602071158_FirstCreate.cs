using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleAudioBooksPlayer.DAL.Migrations
{
    public partial class FirstCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LyricFiles",
                columns: table => new
                {
                    FilePath = table.Column<string>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    ParentFolderName = table.Column<string>(nullable: true),
                    ParentFolderPath = table.Column<string>(nullable: true),
                    DbVersion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LyricFiles", x => x.FilePath);
                });

            migrationBuilder.CreateTable(
                name: "MusicFiles",
                columns: table => new
                {
                    FilePath = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    ParentFolderName = table.Column<string>(nullable: true),
                    ParentFolderPath = table.Column<string>(nullable: true),
                    DbVersion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicFiles", x => x.FilePath);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LyricFiles");

            migrationBuilder.DropTable(
                name: "MusicFiles");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleAudioBooksPlayer.DAL.Migrations
{
    public partial class FirstCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupItems",
                columns: table => new
                {
                    Index = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    HasCover = table.Column<bool>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupItems", x => x.Index);
                });

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
                    Index = table.Column<int>(nullable: true),
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
                    table.ForeignKey(
                        name: "FK_MusicFiles_GroupItems_Index",
                        column: x => x.Index,
                        principalTable: "GroupItems",
                        principalColumn: "Index",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MusicFiles_Index",
                table: "MusicFiles",
                column: "Index");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LyricFiles");

            migrationBuilder.DropTable(
                name: "MusicFiles");

            migrationBuilder.DropTable(
                name: "GroupItems");
        }
    }
}

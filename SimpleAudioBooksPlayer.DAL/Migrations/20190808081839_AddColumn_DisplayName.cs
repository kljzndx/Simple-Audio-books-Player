﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleAudioBooksPlayer.DAL.Migrations
{
    public partial class AddColumn_DisplayName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "SubtitleFiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "MusicFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "SubtitleFiles");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "MusicFiles");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;

namespace SimpleAudioBooksPlayer.DAL
{
    public class FilesContext : DbContext
    {
        public DbSet<MusicFile> MusicFiles { get; set; }
        public DbSet<SubtitleFile> SubtitleFiles { get; set; }
        public DbSet<FileGroup> FileGroups { get; set; }
        public DbSet<PlaybackRecord> PlaybackRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=files.db");
        }
    }
}

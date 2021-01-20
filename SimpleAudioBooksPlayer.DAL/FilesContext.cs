using System;
using Microsoft.EntityFrameworkCore;

namespace SimpleAudioBooksPlayer.DAL
{
    public class FilesContext : DbContext
    {
        public DbSet<FileGroup> FileGroups { get; set; }
        public DbSet<ClassItem> ClassList { get; set; }
        public DbSet<PlaybackRecord> PlaybackRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=files.db");
        }
    }
}

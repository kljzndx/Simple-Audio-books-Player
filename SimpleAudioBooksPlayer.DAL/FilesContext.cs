using System;
using Microsoft.EntityFrameworkCore;

namespace SimpleAudioBooksPlayer.DAL
{
    public class FilesContext : DbContext
    {
        public DbSet<MusicFile> MusicFiles { get; set; }
        public DbSet<LyricFile> LyricFiles { get; set; }
        public DbSet<MusicGroup> GroupItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=files.db");
        }
    }
}

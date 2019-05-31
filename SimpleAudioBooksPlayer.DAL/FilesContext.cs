using System;
using Microsoft.EntityFrameworkCore;

namespace SimpleAudioBooksPlayer.DAL
{
    public class FilesContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=files.db");
        }
    }
}

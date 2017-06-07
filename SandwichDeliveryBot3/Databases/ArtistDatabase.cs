using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SandwichDeliveryBot.SandwichClass;
using Discord;
using SandwichDeliveryBot.OrderStatusEnum;

namespace SandwichDeliveryBot.Databases
{
    public class ArtistDatabase : DbContext
    {
        public DbSet<Sandwich> Artists { get; set; }
        public ArtistDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "artists.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }


    }
}
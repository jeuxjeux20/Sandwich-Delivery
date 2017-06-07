using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SandwichDeliveryBot.SandwichClass;
using Discord;
using SandwichDeliveryBot.OrderStatusEnum;
using SandwichDeliveryBot.ArtistClass;

namespace SandwichDeliveryBot.Databases
{
    public class ArtistDatabase : DbContext
    {
        public DbSet<Artist> Artists { get; set; }

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

        public async Task<Artist> FindArtist(ulong id)
        {
            var a = await Artists.FirstOrDefaultAsync(x => x.ArtistId == id);
            if (a != null) { return a; } else { return null; }
        }
        public async Task<Artist> FindArtist(IGuildUser user)
        {
            var a = await Artists.FirstOrDefaultAsync(x => x.ArtistId == user.Id);
            if (a != null) { return a; } else { return null; }
        }

        public async Task NewArtist(IGuildUser user, string now)
        {
            Artist a = new Artist(user, now);
            await Artists.AddAsync(a);
        }

        public async Task DelArtist(IGuildUser user)
        {
            var a = await Artists.FirstOrDefaultAsync(x => x.ArtistId == user.Id);
            if (a != null)
                Artists.Remove(a);
            else
                throw new CantFindInDatabaseException();
        }
        public void DelArtist(Artist artist)
        {
            if (artist != null)
                Artists.Remove(artist);
            else
                throw new CantFindInDatabaseException();
        }
        public async Task DelArtist(ulong id)
        {
            var a = await Artists.FirstOrDefaultAsync(x => x.ArtistId == id);
            if (a != null)
                Artists.Remove(a);
            else
                throw new CantFindInDatabaseException();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using SandwichDeliveryBot3.CustomClasses;
using SandwichDeliveryBot3.Enums;

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
        public async Task<Artist> FindArtist(string name)
        {
            var a = await Artists.FirstOrDefaultAsync(x => x.ArtistName+"#"+x.ArtistDistin == name);
            if (a != null) { return a; } else { return null; }
        }
        public async Task NewArtist(IGuildUser user, string now)
        {
            Artist a = new Artist(user, now);
            await Artists.AddAsync(a);
            await SaveChangesAsync();
        }

        public async Task DelArtistAsync(IGuildUser user)
        {
            var a = await Artists.FirstOrDefaultAsync(x => x.ArtistId == user.Id);
            if (a != null)
            {
                Artists.Remove(a);
                await SaveChangesAsync();
            }
            else
                throw new CantFindInDatabaseException();
        }
        public async Task ChangeAcceptCount(Artist a, ArtistStatChange c)
        {
            switch (c)
            {
                case ArtistStatChange.Decrease:
                    break;
                case ArtistStatChange.Increase:
                   Artist temp = await Artists.FirstOrDefaultAsync(x => x.ArtistId == a.ArtistId);
                    temp.ordersAccepted += 1;
                    await SaveChangesAsync();
                    break;
            }
        }
        public async Task ChangeDenyCount(Artist a)
        {
            Artist temp = await Artists.FirstOrDefaultAsync(x => x.ArtistId == a.ArtistId);
            temp.ordersDenied += 1;
            await SaveChangesAsync();
        }
        public async Task DelArtistAsync(Artist artist)
        {
            if (artist != null)
            {
                Artists.Remove(artist);
                await SaveChangesAsync();
            }
            else
                throw new CantFindInDatabaseException();
        }
        public async Task DelArtistAsync(ulong id)
        {
            var a = await Artists.FirstOrDefaultAsync(x => x.ArtistId == id);
            if (a != null)
            {
                Artists.Remove(a);
                await SaveChangesAsync();
            }
            else
                throw new CantFindInDatabaseException();
        }
        public async Task UpdateMostRecentOrder(Artist a)
        {
            a.lastOrder = DateTime.Now;
            await SaveChangesAsync();
        }
    }
}
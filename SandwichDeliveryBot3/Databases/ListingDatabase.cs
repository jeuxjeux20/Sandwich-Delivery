using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using Discord;
using SandwichDeliveryBot.OrderStatusEnum;
using SandwichDeliveryBot3.CustomClasses;
using System.Threading.Tasks;

namespace SandwichDeliveryBot.Databases
{
    public class ListingDatabase : DbContext
    {
        public DbSet<Listing> Listings { get; set; }
        public ListingDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "listing.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public async Task NewListing(ListingType t, ulong id, string n, string r)
        {
            Listing list = new Listing(r, t, id, n);
            await Listings.AddAsync(list);
            await SaveChangesAsync();
        }

        public async Task<string> CheckForBlacklist(ulong id) {
            Listing l = await Listings.FirstOrDefaultAsync(x => x.ID == id);
            if (l != null)
            {
                switch (l.Type)
                {
                    case ListingType.User:
                        return "You have been blacklisted.";
                    case ListingType.Guild:
                        return "Your server has been blacklisted.";
                }
            }
            else
            {
                return null;
            }
            return null;
        }

        public async Task<Listing[]> GetArray()
        { 
        return await Listings.ToArrayAsync();
        }
    }
}
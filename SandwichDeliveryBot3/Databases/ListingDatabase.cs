using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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

        public async Task NewListing(ulong id, string n, string r, ListingType t = ListingType.Undefined)
        {
            Listing list = new Listing(r, id, n, t);
            await Listings.AddAsync(list);
            await SaveChangesAsync();
        }

        public async Task RemoveListing(ulong id)
        {
            Listing l = await Listings.FirstOrDefaultAsync(x => x.ID == id);
            if (l != null)
            {
                Listings.Remove(l);
                await SaveChangesAsync();
            }
            else
            {
                throw new CantFindInDatabaseException();
            }
        }

        public async Task RemoveListing(int casen)
        {
            Listing l = await Listings.FirstOrDefaultAsync(x => x.Case == casen);
            if (l != null)
            {
                Listings.Remove(l);
                await SaveChangesAsync();
            }
            else
            {
                throw new CantFindInDatabaseException();
            }
        }

        public async Task EditListing(int casen, string r, string type)
        {
            Listing list = await Listings.FirstOrDefaultAsync(x => x.Case == casen);
            if (list != null)
            {
                list.Reason = r;
                switch (type.ToLower())
                {
                    case "user":
                        list.Type = ListingType.User;
                        break;
                    case "server":
                        list.Type = ListingType.Guild;
                        break;
                    case "guild":
                        list.Type = ListingType.Guild;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                throw new CantFindInDatabaseException();
            }
        }

        public async Task<string> CheckForBlacklist(ulong id) {
            Listing l = await Listings.FirstOrDefaultAsync(x => x.ID == id);
            if (l != null)
            {
                switch (l.Type)
                {
                    case ListingType.User:
                        return "You have been blacklisted for "+l.Reason;
                    case ListingType.Guild:
                        return "Your server has been blacklisted for "+l.Reason;
                    case ListingType.Undefined:
                        return "Either you or this server has been blacklisted for " + l.Reason + ", If you wish to know for sure. Run `;server` and join the invite link.";
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
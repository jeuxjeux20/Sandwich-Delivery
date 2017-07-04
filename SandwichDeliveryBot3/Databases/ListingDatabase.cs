using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using SandwichDeliveryBot3.CustomClasses;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;

namespace SandwichDeliveryBot.Databases
{
    public class ListingDatabase : DbContext
    {
        public DbSet<Listing> Listings { get; set; }
        private IServiceProvider _provider;
        public ListingDatabase(IServiceProvider provider)
        {
            _provider = provider;
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
            DiscordSocketClient c = _provider.GetService<DiscordSocketClient>();
            Listing list = new Listing(r, id,"Undefined", t);
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

        public async Task EditListing(int casen, string n, string r, string type)
        {
            Listing list = await Listings.FirstOrDefaultAsync(x => x.Case == casen);
            if (list != null)
            {
                list.Reason = r;
                list.Name = n;
                switch (type.ToLower())
                {
                    case "user":
                        list.Type = ListingType.User;
                        await SaveChangesAsync();
                        break;
                    case "server":
                        list.Type = ListingType.Guild;
                        await SaveChangesAsync();
                        break;
                    case "guild":
                        list.Type = ListingType.Guild;
                        await SaveChangesAsync();
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


        public async Task EditListing(ulong id, string n, string r, string type)
        {
            Listing list = await Listings.FirstOrDefaultAsync(x => x.ID == id);
            if (list != null)
            {
                list.Reason = r;
                list.Name = n;
                switch (type.ToLower())
                {
                    case "user":
                        list.Type = ListingType.User;
                        await SaveChangesAsync();
                        break;
                    case "server":
                        list.Type = ListingType.Guild;
                        await SaveChangesAsync();
                        break;
                    case "guild":
                        list.Type = ListingType.Guild;
                        await SaveChangesAsync();
                        break;
                    default:
                        await SaveChangesAsync();
                        break;
                }
            }
            else
            {
                throw new CantFindInDatabaseException();
            }
        }

        public async Task EditListing(ulong id, string n)
        {
            Listing list = await Listings.FirstOrDefaultAsync(x => x.ID == id);
            list.Name = n;
            await SaveChangesAsync();
        }

        public async Task EditListing(ulong id, int num)
        {
            Listing list = await Listings.FirstOrDefaultAsync(x => x.ID == id);
            list.Case = num;
            await SaveChangesAsync();
        }


        public async Task<string> CheckForBlacklist(ulong id) {
            Listing l = await Listings.FirstOrDefaultAsync(x => x.ID == id);
            if (l != null)
            {
                switch (l.Type)
                {
                    case ListingType.User:
                        return "You have been blacklisted for `"+l.Reason+"`.";
                    case ListingType.Guild:
                        return "Your server has been blacklisted for `"+l.Reason+"`.";
                    case ListingType.Undefined:
                        return "Either you or this server has been blacklisted for reason: `" + l.Reason + "`, If you wish to know for sure. Run `;server` and join the invite link.";
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
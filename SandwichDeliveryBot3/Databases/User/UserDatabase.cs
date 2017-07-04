using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using SandwichDeliveryBot3.CustomClasses;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using Discord;
using System.Linq;
using SandwichDeliveryBot3.Enums;

namespace SandwichDeliveryBot.Databases
{
    public class UserDatabase : DbContext
    {
        public DbSet<SandwichUser> Users { get; set; }
        private IServiceProvider _provider;
        private ArtistDatabase _ADB;
        public UserDatabase(IServiceProvider provider)
        {
            _provider = provider;
            _ADB = _provider.GetService<ArtistDatabase>();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "users.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public async Task<SandwichUser> FindUser(ulong id)
        {
            var a = await Users.FirstOrDefaultAsync(x => x.Id == id);
            if (a != null) { return a; } else { return null; }
        }
        public async Task CreateNewUser(SocketUser us)
        {
            Console.WriteLine($"Creating a new user profile for {us.Username}.");
            SandwichUser u = new SandwichUser(us);
            await Users.AddAsync(u);
            await SaveChangesAsync();
        }
        public async Task CreateNewUser(IGuildUser us)
        {
            Console.WriteLine($"Creating a new user profile for {us.Username}.");
            SandwichUser u = new SandwichUser(us);
            await Users.AddAsync(u);
            await SaveChangesAsync();
        }
        public async Task ToggleUserType(ulong id)
        {
            var a = await Users.FirstOrDefaultAsync(x => x.Id == id);
            switch (a.Type)
            {
                case UserType.Artist:
                    a.Type = UserType.Customer;
                    await SaveChangesAsync();
                    break;
                case UserType.Customer:
                    a.Type = UserType.Artist;
                    await SaveChangesAsync();
                    break;
            }
        }
        public async Task UpOrders(ulong id)
        {
            var a = await Users.FirstOrDefaultAsync(x => x.Id == id);
            a.Orders += 1;
            await SaveChangesAsync();
        }
        public async Task UpDenials(ulong id)
        {
            var a = await Users.FirstOrDefaultAsync(x => x.Id == id);
            a.Denials += 1;
            await SaveChangesAsync();
        }
        public async Task UpdateRank(SandwichUser user)
        {
            switch (user.Rank)
            {
                case UserRank.NewCustomer:
                    if (user.Orders >= 15)
                    {
                        await AddTips(user, 3);
                        await PromoteUser(user);
                    }
                    break;
                case UserRank.Customer:
                    if (user.Orders >= 30)
                    {
                        await AddTips(user, 10);
                        await PromoteUser(user);
                    }
                    break;
                case UserRank.Regular:
                    if (user.Orders >= 75)
                    {
                        await AddTips(user, 20);
                        await PromoteUser(user);
                    }
                    break;
                case UserRank.SandwichLover:
                    if (user.Orders >= 125)
                    {
                        await AddTips(user, 50);
                        await PromoteUser(user);
                    }
                    break;
                case UserRank.SandwichAddict:
                    if (user.Orders >= 200)
                    {
                        await AddTips(user, 100);
                        await PromoteUser(user);
                    }
                    break;
                case UserRank.InterventionNeeded:
                    if (user.Orders >= 500)
                    {
                        await AddTips(user, 9000);
                        await PromoteUser(user);
                    }
                    break;
                case UserRank.AllHopeLost:
                    //do nothing
                    break;
            }
        }
        public async Task PromoteUser(SandwichUser user)
        {
            int v = (int)user.Rank;
            if (v != 6)
                v += 1;
            else
                return;
            user.Rank = (UserRank)v;
            await SaveChangesAsync();
        }
        public async Task ChangeOrders(ulong id, int c)
        {
            SandwichUser us = await FindUser(id);
            if (us != null)
            {
                us.Orders = c;
                await SaveChangesAsync();
            }
        }
        public async Task ChangeDenials(ulong id, int c)
        {
            SandwichUser us = await FindUser(id);
            if (us != null)
            {
                us.Denials = c;
                await SaveChangesAsync();
            }
        }
        public async Task ChangeRank(ulong id, UserRank r)
        {
            SandwichUser us = await FindUser(id);
            if (us != null)
            {
                us.Rank = r;
                await SaveChangesAsync();
            }
        }
        public async Task AddTips(SandwichUser user, int amount)
        {
            SandwichUser u = await Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            u.Tips += amount;
            await SaveChangesAsync();
        }
        public async Task ChangeTips(SandwichUser send, SandwichUser rec)
        {
            var s = await Users.FirstOrDefaultAsync(x => x.Id == send.Id);
            var a = await _ADB.FindArtist(rec.Id);
            if (a != null)
            {
                s.Tips -= 1;
                a.tipsRecieved += 1;
                await _ADB.SaveChangesAsync();
                await SaveChangesAsync();
            }
        }
    }
}

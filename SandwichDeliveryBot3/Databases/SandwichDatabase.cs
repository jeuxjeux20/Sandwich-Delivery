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
    public class SandwichDatabase : DbContext
    {
        public DbSet<Sandwich> Sandwiches { get; set; }
        public SandwichDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "sandwiches.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public async Task NewOrder(string d, string id, DateTime dat, OrderStatus s, ICommandContext context)
        {
            var order = new Sandwich(d, id, dat, s, context);
            await Sandwiches.AddAsync(order);
            await SaveChangesAsync();
        }

        public bool CheckForExistingOrders(ulong id)
        {
            var s = Sandwiches.FirstOrDefault(x => x.UserId == id);
            if (s != null) { return true; } else { return false; }
        }

        public string VerifyIdUniqueness(string id)
        {
            var s = Sandwiches.FirstOrDefault(x => x.Id == id);
            string newid;
            var r = new Random();
            if (s != null)
            {
                newid = GenerateId(4);
                return newid;
            }
            else
            {
                return id;
            }
        }

        public string GenerateId(int Size)
        {
            string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var chars = Enumerable.Range(0, Size)
                                   .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }

        public async Task DelOrder(string id)
        {
            var s = await Sandwiches.FirstOrDefaultAsync(x => x.Id == id);
            if (s != null)
            {
                Sandwiches.Remove(s);
                await SaveChangesAsync();
            }
        }

        public async Task<Sandwich> FindOrder(ulong userid)
        {
            var s = await Sandwiches.FirstOrDefaultAsync(x => x.UserId == userid);
            if (s != null)
            {
                return s;
            }
            return null;
        }

        //deny order

        //deliver

        //acceptorder

        //order

        //orderinfo

        //gao
    }
}
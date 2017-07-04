using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using SandwichDeliveryBot3.CustomClasses;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using Discord;

namespace SandwichDeliveryBot.Databases
{
    public class TipDatabase : DbContext
    {
        public DbSet<Tip> Tips { get; set; }
        private IServiceProvider _provider;
        private UserDatabase _udb;
        public TipDatabase(IServiceProvider provider)
        {
            _provider = provider;
            _udb = _provider.GetService<UserDatabase>();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "tips.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public async Task NewTip(string sender, string reciever)
        {
            Tip t = new Tip(sender, reciever);
            await Tips.AddAsync(t);
            await SaveChangesAsync();
        }
    }
}
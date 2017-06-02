using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SandwichDeliveryBot.WarningClass;
using Discord;

namespace SandwichDeliveryBot.WarningDB
{
    public class WarningDatabase : DbContext
    {
        public DbSet<Warning> Warnings { get; set; }
        public WarningDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "warnings.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public async Task CreateNewWarning(string reason, ulong id, ulong guildid, string name)
        {
            var warning = new Warning()
            {
                GuildId = guildid,
                UserID = id,
                UserName = name,
                Reason = reason,
                date = DateTime.Now,
                Case = Warnings.Count() + 1
            };
            await Warnings.AddAsync(warning);
            await SaveChangesAsync();
        }

        public Task<Warning[]> GetWarningsOnUserId(ulong id)
            => Warnings.Where(x => x.UserID == id).ToArrayAsync();

        public Task<Warning[]> GetWarningsOnUser(string name)
            => Warnings.Where(x => x.UserName.ToLower() == name.ToLower()).ToArrayAsync();

        public Task<Warning[]> GetWarningsOnUser(IGuildUser user)
        => Warnings.Where(x => x.UserName.ToLower() == user.Username.ToLower() + "#" + user.Discriminator).ToArrayAsync();

        public Task<Warning[]> GetWarnings()
            => Warnings.ToArrayAsync();

        //public async Task DeleteWarningId( ulong id)
        //{
        //    var tag = await Warnings.FirstOrDefaultAsync(x => x.UserID == id);

        //    if (tag == null)
        //        throw new ArgumentException("This id doesnt have any warnings!");

        //    Warnings.Remove(tag);
        //    await SaveChangesAsync();
        //}

        public async Task DeleteWarning(string name)
        {
            var tag = await Warnings.FirstOrDefaultAsync(x => x.UserName.ToLower() == name.ToLower());

            if (tag == null)
                throw new ArgumentException("This name doesnt have any warnings!");

            Warnings.Remove(tag);
            await SaveChangesAsync();
        }

    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using RequireBlacklistPrecon;
using inUSRPrecon;
using NotBlacklistedPreCon;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot.ArtistClass;
using SandwichDeliveryBot.ArtistStatusEnum;
using SandwichDeliveryBot.Databases;

namespace SandwichDeliveryBot.ArtistMod
{
    [Group("artist")]
    public class ArtistModule : ModuleBase
    {

        SandwichService SS;
        ArtistDatabase ADB;

        public ArtistModule(SandwichService s, ArtistDatabase adb)
        {
            SS = s;
            ADB = adb;
        }

        [Command("new")]
        [Alias("a")]
        [NotBlacklisted]
        [RequireBlacklist]
        public async Task AddArtist(params IGuildUser[] artists)
        {
            int newartists = 0;
            foreach (var artist in artists)
            {
                Artist a = await ADB.FindArtist(artist);
                if (a != null)
                {
                    Artist r = await ADB.FindArtist(Context.User.Id);
                    string n = string.Format(artist.Username + "#" + artist.Discriminator);
                    await ADB.NewArtist(artist, DateTime.Now.ToString("MMMM dd, yyyy"));
                    newartists++;
                }
                else
                {
                    await ReplyAsync($"{artist.Username} is already a Sandwich Artist.");
                }
            }
            await ReplyAsync($"{newartists} new Artists have been added.");
        }
        

        [Command("del")]
        [Alias("d")]
        [NotBlacklisted]
        [RequireBlacklist]
        public async Task DeleteArtist(params IGuildUser[] artists)
        {
            int deletedartist = 0;
            foreach (var artist in artists)
            {
                Artist a = await ADB.FindArtist(artist);
                if (a != null)
                {
                    ADB.DelArtistAsync(a);
                    deletedartist++;
                }
                else
                    throw new CantFindInDatabaseException();
            }
            await ReplyAsync($":thumbsup:, {deletedartist} Artists have been removed.");
        }

        [Command("admin")]
        [NotBlacklisted]
        [Alias("a")]
        [RequireBlacklist]
        public async Task CanBlacklist(params IGuildUser[] user)
        {
            int updatedusers = 0;
            foreach (var artist in user)
            {
                var a = await ADB.FindArtist(artist);
                a.canBlacklist = true;
                updatedusers++;
            }
            await ReplyAsync($":thumbsup:, {updatedusers} Artists have been given Administrator control over the bot.");
        }

        [Command("promote")]
        [NotBlacklisted]
        [Alias("p")]
        [RequireBlacklist]
        public async Task PromoteArtist(IGuildUser chef)
        {
            Artist a = await ADB.FindArtist(chef);
            switch (a.status)
            {
                case ArtistStatus.Trainee:
                    a.status = ArtistStatus.Artist;
                    await ReplyAsync($"Promoted {chef.Username}#{chef.Discriminator} from Trainee to Sandwich Artist");
                    break;
                case ArtistStatus.Artist:
                    a.status = ArtistStatus.MasterArtist;
                    await ReplyAsync($"Promoted {chef.Username}#{chef.Discriminator} from Artist to Master Sandwich Artist");
                    break;
                case ArtistStatus.MasterArtist:
                    a.status = ArtistStatus.GodArtist;
                    await ReplyAsync($"Promoted {chef.Username}#{chef.Discriminator} from Master Sandwich Artist to **GOD** Sandwich Artist");
                    break;
                case ArtistStatus.GodArtist:
                    await ReplyAsync("You cannot promote a user past God Sandwich Artist!");
                    break;
            }
        }

        [Command("stats")]
        [Alias("d")]
        [NotBlacklisted]
        public async Task GetDeliveries(params IGuildUser[] artistss)
        {
            foreach (var chef in artistss)
            {
                Artist c = await ADB.FindArtist(chef);
                await ReplyAsync($"{chef.Mention} has accepted `{c.ordersAccepted}` orders and delivered `{c.ordersDelivered}`. They have been working here since `{c.HiredDate}` and have the `{c.status}` rank. Their blacklist ability is set to {c.canBlacklist}.");
            }
        }

        [Command("list")]
        [NotBlacklisted]
        public async Task listImproved()
        {
            var result = string.Join(", \r\n", ADB.Artists.Select(x => string.Format("{0}, {1}", x.ArtistName, x.status)).ToArray());
            await ReplyAsync("result");
        }
    }
}

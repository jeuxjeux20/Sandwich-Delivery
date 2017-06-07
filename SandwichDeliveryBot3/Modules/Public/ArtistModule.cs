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
                    ADB.DelArtist(a);
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
        public async Task CanBlacklist(IGuildUser user)
        {
            
        }

        [Command("promote")]
        [NotBlacklisted]
        [Alias("p")]
        [RequireBlacklist]
        public async Task PromoteArtist(IGuildUser chef)
        {
            ulong n = chef.Id;
            if (SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value != null)
            {
                if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == n).Value != null)
                {
                    Chef c = SS.chefList.FirstOrDefault(a => a.Value.ChefId == n).Value;
                    Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                    if (s.canBlacklist)
                    {
                        switch (c.status)
                        {
                            case ChefStatus.Trainee:
                                c.status = ChefStatus.Artist;
                                await ReplyAsync($"Promoted {chef.Username}#{chef.Discriminator} from Trainee to Sandwich Artist");
                                SS.LogCommand(Context, "Artist Promote - Trainee to Artist", new string[] { chef.Username });
                                break;
                            case ChefStatus.Artist:
                                c.status = ChefStatus.MasterArtist;
                                await ReplyAsync($"Promoted {chef.Username}#{chef.Discriminator} from Artist to Master Sandwich Artist");
                                SS.LogCommand(Context, "Artist Promote - Artist to Master", new string[] { chef.Username });
                                break;
                            case ChefStatus.MasterArtist:
                                c.status = ChefStatus.GodArtist;
                                await ReplyAsync($"Promoted {chef.Username}#{chef.Discriminator} from Master Sandwich Artist to **GOD** Sandwich Artist");
                                SS.LogCommand(Context, "Artist Promote - Master to God", new string[] { chef.Username });
                                break;
                            case ChefStatus.GodArtist:
                                await ReplyAsync("You cannot promote a user past God Sandwich Artist!");
                                SS.LogCommand(Context, "Artist Promote - God error", new string[] { chef.Username });
                                break;
                        }
                    }
                    //SS.Save();
                }
            }
        }

        [Command("count")]
        [NotBlacklisted]
        [Alias("c")]
        public async Task ChefCount()
        {
            await ReplyAsync($"There are currently {SS.chefList.Count} Sandwich Artists in the database.");
            SS.LogCommand(Context, "Artist Count");
        }

        [Command("stats")]
        [Alias("d")]
        [NotBlacklisted]
        public async Task GetDeliveries(IGuildUser chef)
        {
            ulong n = chef.Id;
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == n).Value != null)
            {
                Chef c = SS.chefList.FirstOrDefault(a => a.Value.ChefId == n).Value;
                await ReplyAsync($"{chef.Mention} has accepted `{c.ordersAccepted}` orders and delivered `{c.ordersDelivered}`. They have been working here since `{c.HiredDate}` and have the `{c.status}` rank. Their blacklist ability is set to {c.canBlacklist}.");
                SS.LogCommand(Context, "Artist Stats", new string[] { chef.Username });
            }
            else
            {
                await ReplyAsync("Failed linq pass.");
            }
        }

        [Command("list")]
        [NotBlacklisted]
        public async Task listImproved()
        {
            var s = string.Join("` \r\n `", SS.chefList.Keys);
            await ReplyAsync("`" + s + "`");
            SS.LogCommand(Context, "Artist List");
        }

        [Command("testattribute")]
        [NotBlacklisted]
        [RequireBlacklist]
        public async Task testattrivute()
        {
            await ReplyAsync("it works");
        }

    }
}

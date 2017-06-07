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
        public async Task AddArtist(IGuildUser artist)
        {
            Artist a = await ADB.FindArtist(artist);
            if (a != null)
            {
                    Artist r = await ADB.FindArtist(Context.User.Id);
                    string n = string.Format(artist.Username + "#" + artist.Discriminator);
                    await ADB.NewArtist(artist, DateTime.Now.ToString("MMMM dd, yyyy")  );
                    await ReplyAsync($"<@{artist.Id}> had been added as a Trainee Sandwich Artist!");
            }
            else
            {
                await ReplyAsync("This user is already a Sandwich Artist.");
            }
        }
        

        [Command("del")]
        [Alias("d")]
        [NotBlacklisted]
        [RequireBlacklist]
        public async Task DeleteChef(IGuildUser chef)
        {
            if (SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    string n = string.Format(chef.Username + "#" + chef.Discriminator);
                    if (!SS.chefList.ContainsKey(n))
                    {
                        await ReplyAsync("An entry for this user doesn't exist!");
                    }
                    else
                    {
                        SS.chefList.Remove(n);
                        await ReplyAsync($"{n} has been removed!");
                        SS.LogCommand(Context, "Artist Del", new string[] { chef.Username });
                        //SS.Save();
                    }
                }
            }
        }

        [Command("listdebug")]
        [NotBlacklisted]
        [RequireBlacklist]
        [Alias("l")]
        public async Task ListChefs()
        {
            return;

            foreach (var obj in SS.chefList)
            {

                var c = obj.Value;
                var col = new Color(36, 78, 145);
                DateTimeOffset parseddate;
                if (DateTimeOffset.TryParse(c.HiredDate, out parseddate))
                    Console.WriteLine("Good to go!");
                else
                    parseddate = DateTime.Now;


                await ReplyAsync("Here is your requested information!", embed: new EmbedBuilder()
                 .AddField(builder =>
                 {
                     builder.Name = "Name";
                     builder.Value = c.ChefName;
                     builder.IsInline = true;
                 })
                 .AddField(builder =>
                 {
                     builder.Name = "Orders Accepted";
                     builder.Value = c.ordersAccepted;
                     builder.IsInline = true;
                 })
                 .AddField(builder =>
                 {
                     builder.Name = "Orders Delivered";
                     builder.Value = c.ordersDelivered;
                     builder.IsInline = true;
                 })
                 .AddField(builder =>
                 {
                     builder.Name = "Rank";
                     builder.Value = c.status;
                     builder.IsInline = true;
                 })
                 .AddField(builder =>
                 {
                     builder.Name = "CanBlacklist?";
                     builder.Value = c.canBlacklist;
                     builder.IsInline = true;
                 })
                 .WithUrl("https://discord.gg/XgeZfE2")
                 .WithColor(col)
                 .WithThumbnailUrl(Context.User.GetAvatarUrl())
                 .WithTitle("Chef info.")
                 .WithTimestamp(parseddate));
            }
        }

        [Command("canblacklist")]
        [NotBlacklisted]
        [Alias("cb")]
        [RequireBlacklist]
        public async Task CanBlacklist(IGuildUser user)
        {
            ulong n = user.Id;
            if (SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value != null)
            {
                if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == n).Value != null)
                {
                    Chef c = SS.chefList.FirstOrDefault(a => a.Value.ChefId == n).Value;
                    Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                    if (s.canBlacklist)
                    {
                        c.canBlacklist = true;
                        await ReplyAsync(":thumbsup:");
                        SS.LogCommand(Context, "Artist Can Blacklist", new string[] { user.Username });
                        //SS.Save();
                    }
                }
                else
                {
                    await ReplyAsync("No can do, not a real person.");
                }
            }
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

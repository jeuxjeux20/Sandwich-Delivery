using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot3.Precons;
using SandwichDeliveryBot.Databases;
using SandwichDeliveryBot3.CustomClasses;
using System;

namespace SandwichDeliveryBot3.SandwichMod
{
    public class SandwichModule : ModuleBase
    {
        SandwichService _SS;
        SandwichDatabase _DB;
        ArtistDatabase _ADB;
        ListingDatabase _LDB;

        public SandwichModule(SandwichService ss, SandwichDatabase sdb, ArtistDatabase adb, ListingDatabase ldb)
        {
            _SS = ss;
            _DB = sdb;
            _ADB = adb;
            _LDB = ldb;
        }

        
        //[Command("respond")]
        //[Alias("r")]
        //[NotBlacklisted]
        //[RequireSandwichArtist]
        //public async Task Respond(int id, [Remainder]string response)
        //{
        //    if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
        //    {
        //        Sandwich order = SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value;
        //        try
        //        {
        //            IGuild g = await Context.Client.GetGuildAsync(order.GuildId);
        //            ITextChannel t = await g.GetTextChannelAsync(order.ChannelId);
        //            Color c = new Color(255, 43, 43);
        //            if (t != null)
        //            {
        //                await t.SendMessageAsync($"<@{order.UserId}>, {Context.User.Username}#{Context.User.Discriminator} from The Kitchen™ as responded to your order! They said this:", embed: new EmbedBuilder()
        //        .AddField(builder =>
        //        {
        //            builder.Name = "Message:";
        //            builder.Value = "```" + response + "```";
        //            builder.IsInline = true;
        //        })
        //         .AddField(builder =>
        //         {
        //             builder.Name = "Your order:";
        //             builder.Value = order.Desc;
        //             builder.IsInline = true;
        //         })
        //        .AddField(builder =>
        //        {
        //            builder.Name = "Respond Back?";
        //            builder.Value = "If you wish to respond use the `;messagekitchen` command! (`;mk` for short). Ex `;mk Sorry about the typo! I want it with cheese!` or `;messagekitchen Hey thanks for the sandwich, I really enjoyed it!`.";
        //            builder.IsInline = true;
        //        })
        //        .WithUrl("https://discord.gg/XgeZfE2")
        //        .WithColor(c)
        //        .WithTitle("Message from The Kitchen™!")
        //        .WithTimestamp(DateTime.Now));
        //                await ReplyAsync($"{Context.User.Mention} Response successfully sent!");
        //                IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
        //                ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
        //                ITextChannel usrclog = await usr.GetTextChannelAsync(SS.usrlogcID);
        //                await usrclog.SendMessageAsync($"{Context.User.Mention} has responded to order `{order.Id}` with message: `{response}`.");
        //                await Context.Message.DeleteAsync();
        //                SS.LogCommand(Context, "Respond", new string[] { id.ToString(), response });
        //            }
        //        }
        //        catch (Exception e)//love me some 'defensive' programming
        //        {
        //            await ReplyAsync($"Contact Fires. ```{e}```");
        //            Console.WriteLine(e);
        //        }
        //    }
        //}

        //[Command("messagekitchen")]
        //[Alias("mk")]
        //[NotBlacklisted]
        //public async Task MessageKitchen([Remainder]string message)
        //{
        //    if (message.Length > 5)
        //    {
        //        IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
        //        ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
        //        ITextChannel usrclog = await usr.GetTextChannelAsync(SS.usrlogcID);
        //        Color c = new Color(95, 62, 242);
        //        await usrc.SendMessageAsync($"New message from {Context.User.Mention}!", embed: new EmbedBuilder()
        //        .AddField(builder =>
        //        {
        //            builder.Name = "Message:";
        //            builder.Value = "```" + message + "```";
        //            builder.IsInline = true;
        //        })
        //         .AddField(builder =>
        //         {
        //             builder.Name = "Guild:";
        //             builder.Value = $"{Context.Guild.Name}({Context.Guild.Id})";
        //             builder.IsInline = true;
        //         })
        //          .AddField(builder =>
        //          {
        //              builder.Name = "Channel:";
        //              builder.Value = $"{Context.Channel.Name}({Context.Channel.Id})";
        //              builder.IsInline = true;
        //          })
        //        .WithColor(c)
        //        .WithTitle("Message from a customer!")
        //        .WithTimestamp(DateTime.Now));


        //        //Too lazy to create an embed and send them to both. so gonna run the shitty way. sorry D:


        //        await usrclog.SendMessageAsync($"New message from {Context.User.Mention}!", embed: new EmbedBuilder()
        //        .AddField(builder =>
        //        {
        //            builder.Name = "Message:";
        //            builder.Value = "```" + message + "```";
        //            builder.IsInline = true;
        //        })
        //         .AddField(builder =>
        //         {
        //             builder.Name = "Guild:";
        //             builder.Value = $"{Context.Guild.Name}({Context.Guild.Id})";
        //             builder.IsInline = true;
        //         })
        //          .AddField(builder =>
        //          {
        //              builder.Name = "Channel:";
        //              builder.Value = $"{Context.Channel.Name}({Context.Channel.Id})";
        //              builder.IsInline = true;
        //          })
        //        .WithColor(c)
        //        .WithTitle("Message from a customer!")
        //        .WithTimestamp(DateTime.Now));
        //        await Context.Message.DeleteAsync();
        //        await ReplyAsync(":thumbsup:");
        //        SS.LogCommand(Context, "Respond", new string[] { message });
        //    }
        //    else
        //    {
        //        await ReplyAsync("Your message must be longer then 5 characters.");
        //    }
        //}

        [Command("motd")]
        [NotBlacklisted]
        public async Task MOTD()
        {
            await ReplyAsync(_SS.motd);
        }

        [Command("blacklist")]
        [Alias("b")]
        [RequireBlacklist]
        public async Task Blacklist(ulong id, string name = "Undefined", [Remainder]string reason = "No reason given.")
        {
          
            Artist a = await _ADB.FindArtist(Context.User.Id);
            if (a != null)
            {
                await _LDB.NewListing(id, name, reason);
                IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
                ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
                await usrc.SendMessageAsync($"{Context.User.Mention} blacklisted <@{id}> for `{reason}`(id).");
                await ReplyAsync(":thumbsup:");
            }
        }

        [Command("blacklist")]
        [Alias("b")]
        [RequireBlacklist]
        public async Task Blacklist(IGuildUser user, [Remainder]string reason = "No reason given.")
        {
            Artist a = await _ADB.FindArtist(Context.User.Id);
            if (a != null)
            {
                await _LDB.NewListing(user.Id, user.Username, reason);
                IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
                ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
                await usrc.SendMessageAsync($"{Context.User.Mention} blacklisted <@{user.Id}> for `{reason}`(id).");
                await ReplyAsync(":thumbsup:");
            }
        }


        [Command("unblacklist")]
        [Alias("ub")]
        [RequireBlacklist]
        public async Task removeFromBlacklist(ulong id)
        {
            await _LDB.RemoveListing(id);
            IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
            await usrc.SendMessageAsync($"{Context.User.Mention} unblacklisted <@{id}>(id).");
            await ReplyAsync(":thumbsup:");
        }

        [Command("unblacklist")]
        [Alias("ub")]
        [RequireBlacklist]
        public async Task removeFromBlacklist(int casen)
        {
            Listing[] a = await _LDB.GetArray();
            Listing list = a.FirstOrDefault(x => x.Case == casen);
            await _LDB.RemoveListing(casen);
            IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
            await usrc.SendMessageAsync($"{Context.User.Mention} unblacklisted <@{list.ID}>(case).");
            await ReplyAsync(":thumbsup:");
        }

        [Command("unblacklist")]
        [Alias("ub")]
        [RequireBlacklist]
        public async Task removeFromBlacklist(IGuildUser user)
        {
            await _LDB.RemoveListing(user.Id);
            IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
            await usrc.SendMessageAsync($"{Context.User.Mention} unblacklisted <@{user.Id}>(user).");
            await ReplyAsync(":thumbsup:");
        }

        [Command("listings")]
        public async Task showListings()
        {
            foreach (var o in _LDB.Listings)
            {
                await ReplyAsync($"{o.Name}, {o.Type}, {o.ID}, {o.Case}");
            }
        }

        [Command("editlisting")]
        public async Task editListings(ulong id, string type, [Remainder]string reason)
        {
            await _LDB.EditListing(id, reason, type);
            IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
            await usrc.SendMessageAsync($"{Context.User.Mention} edited listing, {id}, {type}, {reason}.");
            await ReplyAsync(":thumbsup:");
        }

        [Command("listinginfo")]
        [Alias("li")]
        public async Task listinginfo(ulong id)
        {
            Listing[] list = await _LDB.GetArray();
            Listing listing = list.FirstOrDefault(x => x.ID == id);
            await ReplyAsync($"{Context.User.Mention} Here is your requested information!", embed: new EmbedBuilder()
            .AddField(builder =>
            {
                builder.Name = "Name";
                builder.Value = listing.Name;
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "Reason";
                builder.Value = listing.Reason;
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "User ID";
                builder.Value = listing.ID;
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "Type";
                builder.Value = listing.Type;
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "Case number";
                builder.Value = listing.Case;
                builder.IsInline = true;
            })
             .AddField(builder =>
             {
                 builder.Name = "Date of listing";
                 builder.Value = listing.Date;
                 builder.IsInline = true;
             })
            .WithUrl("https://discord.gg/XgeZfE2")
            .WithTitle("Listing information")
            .WithTimestamp(DateTime.Now));

        }

        [Command("amiblacklisted")]
        public async Task amiblacklisted()
        {
            Console.WriteLine(await _LDB.CheckForBlacklist(Context.User.Id));
            string r = await _LDB.CheckForBlacklist(Context.User.Id);
            Console.WriteLine(r);
            await ReplyAsync(r ?? "lmao failed");
        }

        [Command("totalorders")]
        [Alias("to")]
        [NotBlacklisted]
        public async Task TotalOrders()
        {
            await ReplyAsync($"We have proudly served {_SS.totalOrders} sandwiches since June.");
        }

        [Command("credits")]
        [Alias("cred")]
        [NotBlacklisted]
        public async Task credits()
        {
            await ReplyAsync($"https://github.com/USRDiscordBots/Sandwich-Delivery-Bot-v2.0/wiki/Getting-Started-as-a-Sandwich-Artist#before-you-continue-a-quick-thank-you-to");
        }

        [Command("help")]
        [Alias("h")]
        public async Task Help()
        {
            await ReplyAsync(@"**__COMMANDS__**

        **» ;order**
             Orders a sandwich
             Example usage:
             ;order BLT with extra lettuce

        **» ;feedback**
             Gives our bot some feedback
             Example usage:
             ;feedback I didn't get as much extra lettuce as I would have liked, but it was enough. 
             *Please do not mention our workers in the feedback!*

        **» ;delorder**
             Deletes your previous order

        **» ;motd**
             Sends the message that the bot sends when it first joins a server.

       **» ;totalorders**
             Displays the amount of orders we have done!

        **» ;credits**
             Returns the credits

        **» ;help**
             Displays this message

        **» ;server**
             Gives you an invite to our server!!
         ");
        }


    }
}

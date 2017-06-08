using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using SandwichDeliveryBot.Databases;
using SandwichDeliveryBot.SandwichClass;
using SandwichDeliveryBot.SService;
using NotBlacklistedPreCon;
using SandwichDeliveryBot.OrderStatusEnum;
using RequireSandwichArtistPrecon;
using inUSRPrecon;
using SandwichDeliveryBot.ArtistClass;

namespace SandwichDeliveryBot3.Modules.Public
{
    public class OrderModule : ModuleBase
    {
        SandwichService _SS;
        SandwichDatabase _DB;
        ArtistDatabase _ADB;

        public OrderModule(SandwichService ss, SandwichDatabase sdb, ArtistDatabase adb)
        {
            _SS = ss;
            _DB = sdb;
            _ADB = adb;
        }

        [Command("order")]
        [Alias("o")]
        [NotBlacklisted]
        [Summary("Ogre!")]
        [RequireBotPermission(GuildPermission.CreateInstantInvite)]
        public async Task Order([Remainder]string order)
        {
            if (order.Length > 4)
            {
                var outp = _DB.CheckForExistingOrders(Context.User.Id);
                if (outp) { await ReplyAsync("You already have an order!"); return; }
                REMOVED NULL CHECK, ALREADY DONE BY DISCORD. REPLACED WITH LENGTHHHH
                string orderid;

                MOVED TO PRECONDITION
                if (SS.hasAnOrder.ContainsKey(Context.User.Id)) { await ReplyAsync($"You already haven an order placed! :angry: "); return; } //move to precondition

                try
                {
                    IGuild usr = await Context.Client.GetGuildAsync(264222431172886529); //Isn't there a better way to do these?
                    ITextChannel usrc = await usr.GetTextChannelAsync(307160239646441474);
                    ITextChannel usrclog = await usr.GetTextChannelAsync(287990510428225537);

                    orderid = _DB.GenerateId(5);
                    orderid = _DB.VerifyIdUniqueness(orderid);

                    var neworder = _DB.NewOrder(order, orderid, DateTime.Now, OrderStatus.Waiting, Context);

                    SS.activeOrders.Add(i, o); //_DB.NewOrder using above parameters
                    var builder = new EmbedBuilder();
                    builder.ThumbnailUrl = Context.User.GetAvatarUrl();
                    builder.Title = $" New order from {Context.Guild.Name}(`{Context.Guild.Id}`)";
                    var desc = $"Ordered by: **{Context.User.Username}**#**{Context.User.Discriminator}**(`{Context.User.Id}`)\n" +
                       $"Channel: `{Context.Channel.Name}`\n" +
                       $"Id: `{orderid}`\n" +
                       $"```{order}```";
                    builder.Description = desc;
                    builder.Color = new Color(84, 176, 242);
                    builder.WithFooter(x =>
                    {
                        x.Text = "Is this order abusive? Please tell Lemon or Fires immediately!";
                    });
                    builder.Timestamp = DateTime.Now;

                    var artist = usr.Roles.FirstOrDefault(x => x.Name == "Sandwich rtists"); //FIX
                    if (artist != null)
                    {
                        await usrc.SendMessageAsync($"{artist.Mention}", embed: builder);
                    }
                    else
                    {
                        await usrc.SendMessageAsync($".", embed: builder);
                    }

                }
                catch (Exception e)
                {
                    await ReplyAsync("This error should not happen! Contact Fires#1043 immediately!");
                    Console.WriteLine(e);
                    await ReplyAsync($"```{e}```");
                    return;
                }

                DM USER ABOUT THEIR ORDER

                IDMChannel dm = await Context.User.CreateDMChannelAsync();
                IUserMessage m = await ReplyAsync(":thumbsup:");
                try
                {
                    await dm.SendMessageAsync($"Your order has been delivered! Thank you for ordering! Please wait while someone accepts your order. :slight_smile: - ID `{orderid}`");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await m.ModifyAsync(msg =>
                    {
                        msg.Content = $":thumbsdown: {Context.User.Mention} We failed to dm you. You're order has been automatically deleted. Please enable DMs and re order. http://i.imgur.com/vY7tThf.png OR http://i.imgur.com/EtaA78Q.png";
                    });
                    SS.hasAnOrder.Remove(Context.User.Id);
                    SS.activeOrders.Remove(i);
                    IGuild usr = await Context.Client.GetGuildAsync(_SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(_SS.usrcID);
                    ITextChannel usrclog = await usr.GetTextChannelAsync(_SS.usrlogcID);
                    await usrc.SendMessageAsync($"**IGNORE ORDER {orderid} AS IT HAS BEEN REMOVED**");
                    await usrclog.SendMessageAsync($"Order {orderid} has been removed due to the customer having their dms closed.");
                }
                no need for saving, sqlite does it all :)
                    SS.Save();
            }
            else { await ReplyAsync("Your order must be longer then 5 characters."); }
        }

        [Command("delorder")]
        [Alias("delo")]
        [NotBlacklisted]
        public async Task DelOrder()
        {
            IUserMessage msg = await ReplyAsync("Attempting to delete order...");
            try
            {
                Sandwich order = await _DB.FindOrder(Context.User.Id);
                await _DB.DelOrder(order.Id);
                IGuild usr = await Context.Client.GetGuildAsync(264222431172886529);
                ITextChannel usrc = await usr.GetTextChannelAsync(307160239646441474);
                await usrc.SendMessageAsync($"Order `{order.Id}`,`{order.Desc}` has been **REMOVED**.");
                await msg.ModifyAsync(x =>
                {
                    x.Content = "Successfully deleted order!";
                });
            }
            catch (Exception e)
            {
                await msg.ModifyAsync(x =>
                {
                    x.Content = "Failed to delete. Are you sure you have one? If this issue persists contact Fires#1043.";
                });
                Console.WriteLine(e);
            }
        }

        [Command("acceptorder")]
        [Alias("ao")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task AcceptOrder(string id)
        {
            Artist a = await _ADB.FindArtist(Context.User.Id);
            Sandwich o = await _DB.FindOrder(id);
            if (o.Status == OrderStatus.ReadyToDeliver) { await ReplyAsync("This order is already ready to be delivered! :angry: "); return; }
            using (Context.Channel.EnterTypingState())
            {
                await ReplyAsync($"{Context.User.Mention} Sandwich order is now ready for delivery! Please assemble the sandwich, once you are complete. `;deliver {id}` to continue! Type `;orderinfo {id}`(short: `;oi {id}`) if you need more info. :wave: ");
                IGuild s = await Context.Client.GetGuildAsync(o.GuildId);
                ITextChannel ch = await s.GetTextChannelAsync(o.ChannelId);
                IGuildUser u = await s.GetUserAsync(o.UserId);

                IDMChannel dm = await u.CreateDMChannelAsync();
                var builder = new EmbedBuilder();

                builder.ThumbnailUrl = Context.User.GetAvatarUrl();
                builder.Title = $"Your order has been accepted by {Context.User.Username}#{Context.User.Discriminator}!";
                var desc = $"```{o.Desc}```\n" +
                           $"Id: `{o.Id}`\n" +
                           $"**Watch this chat for an updates on when it is on it's way! It is ready for delivery!";
                builder.Description = desc;
                builder.Color = new Color(36, 78, 145);
                builder.Url = "https://discord.gg/XgeZfE2";
                builder.WithFooter(x =>
                {
                    x.IconUrl = u.GetAvatarUrl();
                    x.Text = $"Ordered at: {o.date}.";
                });
                builder.Timestamp = DateTime.UtcNow;
                o.Status = OrderStatus.ReadyToDeliver;
                a.ordersAccepted += 1;
                await dm.SendMessageAsync("", embed: builder); ;
            }
        }


    }
}
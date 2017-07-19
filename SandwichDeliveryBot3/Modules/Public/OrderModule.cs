using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using SandwichDeliveryBot.Databases;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot.OrderStatusEnum;
using SandwichDeliveryBot3.Precons;
using SandwichDeliveryBot3.CustomClasses;
using SandwichDeliveryBot3.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace SandwichDeliveryBot3.Modules.Public
{
    public class OrderModule : ModuleBase
    {
        SandwichService _SS;
        SandwichDatabase _DB;
        ArtistDatabase _ADB;
        ListingDatabase _LDB;
        UserDatabase _UDB;

        public OrderModule(IServiceProvider provider)
        {
            _SS = provider.GetService<SandwichService>();
            _DB = provider.GetService<SandwichDatabase>();
            _ADB = provider.GetService<ArtistDatabase>();
            _LDB = provider.GetService<ListingDatabase>();
            _UDB = provider.GetService<UserDatabase>();
        }

        [Command("getallorders")]
        [Alias("gao")]
        public async Task Gao()
        {
            var result = string.Join(", \r\n", _DB.Sandwiches.Select(x => string.Format("{0} - `{1}`", x.Id, x.UserName+"#"+x.Discriminator)).ToArray());
            await ReplyAsync(result);
        }

        [Command("order")]
        [Alias("o")]
        [NotBlacklisted]
        [Summary("Ogre!")]
        [RequireBotPermission(GuildPermission.CreateInstantInvite)]
        public async Task Order([Remainder]string order)
        {
            using (Context.Channel.EnterTypingState())
            {
                if (order.Length > 1)
                {
                    var outp = _DB.CheckForExistingOrders(Context.User.Id);
                    if (outp) { await ReplyAsync("You already have an order!"); return; }
                    string orderid;

                    try
                    {
                        IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId); //Isn't there a better way to do these?
                        ITextChannel usrc = await usr.GetTextChannelAsync(_SS.KitchenId);
                        ITextChannel usrclog = await usr.GetTextChannelAsync(_SS.LogId);

                        orderid = _DB.GenerateId(3);
                        orderid = _DB.VerifyIdUniqueness(orderid);

                        var neworder = _DB.NewOrder(order, orderid, DateTime.Now, OrderStatus.Waiting, Context);

                        var builder = new EmbedBuilder();
                        builder.ThumbnailUrl = new Uri(Context.User.GetAvatarUrl());
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
                        _SS.totalOrders += 1;

                        var artist = usr.Roles.FirstOrDefault(x => x.Name.ToLower() == "sandwich artists"); //FIX
                        if (artist != null)
                        {
                            await usrc.SendMessageAsync($"{artist.Mention}", embed: builder);
                        }
                        else
                        {
                            await usrc.SendMessageAsync($" ", embed: builder);
                        }

                    }
                    catch (Exception e)
                    {
                        await ReplyAsync("This error should not happen! Contact Fires#1043 immediately!");
                        Console.WriteLine(e);
                        await ReplyAsync($"```{e}```");
                        return;
                    }

                    IDMChannel dm = await Context.User.GetOrCreateDMChannelAsync();
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
                        IGuild usr = await Context.Client.GetGuildAsync(322455281286119466);
                        ITextChannel usrc = await usr.GetTextChannelAsync(322455717254529034);
                        ITextChannel usrclog = await usr.GetTextChannelAsync(322463971359588352);
                        await usrc.SendMessageAsync($"**IGNORE ORDER {orderid} AS IT HAS BEEN REMOVED**");
                        await usrclog.SendMessageAsync($"Order {orderid} has been removed due to the customer having their dms closed.");
                    }
                }
                else { await ReplyAsync("Your order must be longer then 5 characters."); }
            }
        }

        [Command("delorder")]
        [Alias("delo")]
        [NotBlacklisted]
        public async Task DelOrder()
        {
            using (Context.Channel.EnterTypingState())
            {
                IUserMessage msg = await ReplyAsync("Attempting to delete order...");
                try
                {
                    Sandwich order = await _DB.FindOrder(Context.User.Id);
                    await _DB.DelOrder(order.Id.ToLower());
                    IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
                    ITextChannel usrc = await usr.GetTextChannelAsync(_SS.KitchenId);
                    await usrc.SendMessageAsync($"Order `{order.Id}`,`{order.Desc}` has been **REMOVED**.");
                    _SS.totalOrders -= 1;
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
        }

        [Command("acceptorder")]
        [Alias("ao")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task AcceptOrder(string id)
        {
            using (Context.Channel.EnterTypingState())
            {
                try
                {
                    Artist a = await _ADB.FindArtist(Context.User.Id);
                    Sandwich o = await _DB.FindOrder(id);
                    if (o.Status == OrderStatus.ReadyToDeliver) { await ReplyAsync("This order is already ready to be delivered! :angry: "); return; }

                    await ReplyAsync($"{Context.User.Mention} Sandwich order is now ready for delivery! Please assemble the sandwich, once you are complete. `;deliver {id}` to continue! Type `;orderinfo {id}`(short: `;oi {id}`) if you need more info. :wave: ");
                    IGuild s = await Context.Client.GetGuildAsync(o.GuildId);
                    ITextChannel ch = await s.GetTextChannelAsync(o.ChannelId);
                    IGuildUser u = await s.GetUserAsync(o.UserId);

                    IDMChannel dm = await u.GetOrCreateDMChannelAsync();
                    var builder = new EmbedBuilder();

                    builder.ThumbnailUrl = new Uri(Context.User.GetAvatarUrl());
                    builder.Title = $"Your order has been accepted by {Context.User.Username}#{Context.User.Discriminator}!";
                    var desc = $"```{o.Desc}```\n" +
                               $"Id: `{o.Id}`\n" +
                               $"Watch this chat for an updates on when it is on it's way! It is ready for delivery!";
                    builder.Description = desc;
                    builder.Color = new Color(36, 78, 145);
                    builder.Url = new Uri("https://discord.gg/XgeZfE2");
                    builder.WithFooter(x =>
                    {
                        x.IconUrl = new Uri(u.GetAvatarUrl()); 
                        x.Text = $"Ordered at: {o.date}.";
                    });
                    builder.Timestamp = DateTime.UtcNow;
                    o.Status = OrderStatus.ReadyToDeliver;
                    o.ArtistId = Context.User.Id;
                    await _ADB.ChangeAcceptCount(a, ArtistStatChange.Increase);
                    await dm.SendMessageAsync(" ", embed: builder);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                
            }
        }


        [Command("deliver")]
        [Alias("d")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task Deliver(string id)
        {
            using (Context.Channel.EnterTypingState())
            {
                Sandwich o = await _DB.FindOrder(id);
                if (o.ArtistId == Context.User.Id)
                {
                    if (o.Status == OrderStatus.ReadyToDeliver)
                    {
                        Artist a = await _ADB.FindArtist(Context.User.Id);
                        //Collect variables
                        await ReplyAsync($"{Context.User.Mention} DMing you an invite! Go deliver it! Remember to be nice and ask for `;feedback`.");
                        IGuild s = await Context.Client.GetGuildAsync(o.GuildId);
                        SandwichUser user = await _UDB.FindUser(o.UserId);
                        await _UDB.UpOrders(user.Id);
                        ITextChannel ch = await s.GetTextChannelAsync(o.ChannelId);
                        IGuildUser u = await s.GetUserAsync(o.UserId);
                        IDMChannel dm = await u.GetOrCreateDMChannelAsync();
                        //Create Invite
                        IInvite inv = await ch.CreateInviteAsync(0, 1, false, true);
                        IDMChannel artistdm = await Context.User.GetOrCreateDMChannelAsync();
                        //Build embed
                        var builder = new EmbedBuilder();
                        builder.ThumbnailUrl = new Uri(o.AvatarUrl);
                        builder.Title = $"Your order is being delivered by {Context.User.Username}#{Context.User.Discriminator}!";
                        var desc = $"```{o.Desc}```\n" +
                                   $"**Incoming sandwich! Watch {o.GuildName}!**";
                        builder.Description = desc;
                        builder.Color = new Color(163, 198, 255);
                        builder.WithFooter(x =>
                        {
                            x.IconUrl = new Uri(u.GetAvatarUrl());
                            x.Text = $"Ordered at: {o.date}.";
                        });
                        builder.Timestamp = DateTime.UtcNow;
                        await dm.SendMessageAsync($"Your sandwich is being delivered soon! Watch out!", embed: builder);
                        //Finish up
                        await artistdm.SendMessageAsync("Invite: " + inv.ToString() +" \r\n Name: "+o.UserName);
                        o.Status = OrderStatus.Delivered;
                        await _ADB.UpdateMostRecentOrder(a);
                        await _UDB.UpdateRank(user);
                        await _DB.DelOrder(id);
                        //await e.Channel.SendMessage("The Order has been completed and removed from the system. You cannot go back now!");

                    }
                    else
                    {
                        await ReplyAsync("This order is not ready to be delivered yet.");
                    }
                }
                else
                {
                    await ReplyAsync("You have not claimed this order!");
                }
            }
        }

        [Command("denyorder")]
        [Alias("do")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task DenyOrder(string id, [Remainder] string reason)
        {
            try
            {
                Sandwich order = await _DB.FindOrder(id);
                order.Status = OrderStatus.Delivered;
                await _DB.DelOrder(id);
                await ReplyAsync($"{Context.User.Mention} Deleted order {order.Id}!");
                IGuild s = await Context.Client.GetGuildAsync(order.GuildId);
                ITextChannel ch = await s.GetTextChannelAsync(order.ChannelId);
                IGuildUser u = await s.GetUserAsync(order.UserId);
                IDMChannel dm = await u.GetOrCreateDMChannelAsync();
                Artist a = await _ADB.FindArtist(Context.User.Id);
                await _ADB.ChangeAcceptCount(a, ArtistStatChange.Decrease);
                await _ADB.ChangeDenyCount(a);
                SandwichUser user = await _UDB.FindUser(order.UserId);
                await _UDB.UpDenials(user.Id);
                await dm.SendMessageAsync($"Your sandwich order has been denied! ", embed: new EmbedBuilder()
                    .WithThumbnailUrl(new Uri(Context.User.GetAvatarUrl()))
                    .WithUrl(new Uri("https://discord.gg/XgeZfE2"))
                    .AddField(builder =>
                    {
                        builder.Name = "Order:";
                        builder.Value = order.Desc;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Denied By:";
                        builder.Value = string.Join("#", Context.User.Username, Context.User.Discriminator);
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Denied because:";
                        builder.Value = reason;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Ordered at:";
                        builder.Value = order.date;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Server:";
                        builder.Value = order.GuildName;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Order Status:";
                        builder.Value = order.Status;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Order Id:";
                        builder.Value = order.Id;
                        builder.IsInline = true;
                    })
                    .WithCurrentTimestamp()
                    .WithTitle("Denied order:"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("orderinfo")]
        [Alias("oi")]
        [NotBlacklisted]
        [RequireSandwichArtist]
        public async Task OrderInfo(string id)
        {
            Sandwich order = await _DB.FindOrder(id);
            Artist art = await _ADB.FindArtist(order.ArtistId);
                Color c = new Color(102, 102, 153);
                await ReplyAsync($"{Context.User.Mention} Here is your requested information!", embed: new EmbedBuilder()
                .AddField(builder =>
                {
                    builder.Name = "Order";
                    builder.Value = order.Desc;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Artist";
                    if (art != null)
                    {
                        builder.Value = art.ArtistName+"#"+art.ArtistDistin;
                    }
                    else
                    {
                        builder.Value = "None";
                    }
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Order Id";
                    builder.Value = order.Id;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Order Server";
                    builder.Value = order.GuildName;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Order Date";
                    builder.Value = order.date;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Customer";
                    builder.Value = order.UserName + "#" + order.Discriminator;
                    builder.IsInline = true;
                })
                 .AddField(builder =>
                 {
                     builder.Name = "Order Status";
                     builder.Value = order.Status;
                     builder.IsInline = true;
                 })
                .WithUrl(new Uri("https://discord.gg/XgeZfE2"))
                .WithColor(c)
                .WithThumbnailUrl(new Uri(order.AvatarUrl))
                .WithTitle("Order information")
                .WithTimestamp(DateTime.Now));
            
        }

        [Command("feedback")]
        [Alias("f")]
        [NotBlacklisted]
        public async Task Feedback([Remainder]string f)
        {
            if (f != null)
            {
                try
                {
                    IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
                    ITextChannel usrc = await usr.GetTextChannelAsync(306941357795311617);

                    var builder = new EmbedBuilder();                    
                    builder.ThumbnailUrl = new Uri(Context.User.GetAvatarUrl() ?? new Uri("http://www.mamul.am/images/avatars/user-unknown-icon.jpg"));                                   
                    builder.Title = $"New feedback from {Context.User.Username}#{Context.User.Discriminator}(`{Context.User.Id}`)";
                    var desc = $"{f}";
                    builder.Description = desc;
                    builder.Color = new Color(242, 255, 5);
                    builder.WithFooter(x =>
                    {
                        x.Text = "Is this feedback abusive? Please tell Lemon or Fires immediately!";
                    });
                    builder.Timestamp = DateTime.Now;

                    await usrc.SendMessageAsync(" ", embed: builder);
                    await ReplyAsync("Thank you!");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Error! {e}");
                }
            }
            else
            {
                await ReplyAsync("Please enter something!");
            }
        }
    }
}

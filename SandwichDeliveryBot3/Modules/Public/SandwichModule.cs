using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using RequireSandwichArtistPrecon;
using RequireBlacklistPrecon;
using inUSRPrecon;
using NotBlacklistedPreCon;
using System.Collections.Generic;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot.SandwichClass;
using SandwichDeliveryBot.OrderStatusEnum;
using SandwichDeliveryBot.ChefClass;

namespace SandwichDeliveryBot3.SandwichMod
{
    public class SandwichModule : ModuleBase
    {
        SandwichService SS;
        public SandwichModule(SandwichService s)
        {
            SS = s;
        }
        
        [Command("getordercount")]
        public async Task GetOrderCount()
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync($"{Context.User.Mention} We have served `{SS.totalOrders}`");
            SS.LogCommand(Context, "Get Order Count");
        }

        [Command("getallorders")]
        [Alias("gao")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task GetAllOrders()
        {
            var s = string.Join("` \r\n `", SS.activeOrders.Keys);
            await Context.Message.DeleteAsync();
            await ReplyAsync($"{Context.User.Mention} `{s}`");
            SS.LogCommand(Context, "Get All Orders");
        }

        [Command("respond")]
        [Alias("r")]
        [NotBlacklisted]
        [RequireSandwichArtist]
        public async Task Respond(int id, [Remainder]string response)
        {
            if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
            {
                Sandwich order = SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value;
                try
                {
                    IGuild g = await Context.Client.GetGuildAsync(order.GuildId);
                    ITextChannel t = await g.GetTextChannelAsync(order.ChannelId);
                    Color c = new Color(255, 43, 43);
                    if (t != null)
                    {
                        await t.SendMessageAsync($"<@{order.UserId}>, {Context.User.Username}#{Context.User.Discriminator} from The Kitchen™ as responded to your order! They said this:", embed: new EmbedBuilder()
                .AddField(builder =>
                {
                    builder.Name = "Message:";
                    builder.Value = "```" + response + "```";
                    builder.IsInline = true;
                })
                 .AddField(builder =>
                 {
                     builder.Name = "Your order:";
                     builder.Value = order.Desc;
                     builder.IsInline = true;
                 })
                .AddField(builder =>
                {
                    builder.Name = "Respond Back?";
                    builder.Value = "If you wish to respond use the `;messagekitchen` command! (`;mk` for short). Ex `;mk Sorry about the typo! I want it with cheese!` or `;messagekitchen Hey thanks for the sandwich, I really enjoyed it!`.";
                    builder.IsInline = true;
                })
                .WithUrl("https://discord.gg/XgeZfE2")
                .WithColor(c)
                .WithTitle("Message from The Kitchen™!")
                .WithTimestamp(DateTime.Now));
                        await ReplyAsync($"{Context.User.Mention} Response successfully sent!");
                        IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                        ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
                        ITextChannel usrclog = await usr.GetTextChannelAsync(SS.usrlogcID);
                        await usrclog.SendMessageAsync($"{Context.User.Mention} has responded to order `{order.Id}` with message: `{response}`.");
                        await Context.Message.DeleteAsync();
                        SS.LogCommand(Context, "Respond",new string[] {id.ToString(),response });
                    }
                }
                catch (Exception e)//love me some 'defensive' programming
                {
                    await ReplyAsync($"Contact Fires. ```{e}```");
                    Console.WriteLine(e);
                }
            }
        }

        [Command("messagekitchen")]
        [Alias("mk")]
        [NotBlacklisted]
        public async Task MessageKitchen([Remainder]string message)
        {
            if (message.Length > 5)
            {
                IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
                ITextChannel usrclog = await usr.GetTextChannelAsync(SS.usrlogcID);
                Color c = new Color(95, 62, 242);
                await usrc.SendMessageAsync($"New message from {Context.User.Mention}!", embed: new EmbedBuilder()
                .AddField(builder =>
                {
                    builder.Name = "Message:";
                    builder.Value = "```" + message + "```";
                    builder.IsInline = true;
                })
                 .AddField(builder =>
                 {
                     builder.Name = "Guild:";
                     builder.Value = $"{Context.Guild.Name}({Context.Guild.Id})";
                     builder.IsInline = true;
                 })
                  .AddField(builder =>
                  {
                      builder.Name = "Channel:";
                      builder.Value = $"{Context.Channel.Name}({Context.Channel.Id})";
                      builder.IsInline = true;
                  })
                .WithColor(c)
                .WithTitle("Message from a customer!")
                .WithTimestamp(DateTime.Now));


                //Too lazy to create an embed and send them to both. so gonna run the shitty way. sorry D:


                await usrclog.SendMessageAsync($"New message from {Context.User.Mention}!", embed: new EmbedBuilder()
                .AddField(builder =>
                {
                    builder.Name = "Message:";
                    builder.Value = "```" + message + "```";
                    builder.IsInline = true;
                })
                 .AddField(builder =>
                 {
                     builder.Name = "Guild:";
                     builder.Value = $"{Context.Guild.Name}({Context.Guild.Id})";
                     builder.IsInline = true;
                 })
                  .AddField(builder =>
                  {
                      builder.Name = "Channel:";
                      builder.Value = $"{Context.Channel.Name}({Context.Channel.Id})";
                      builder.IsInline = true;
                  })
                .WithColor(c)
                .WithTitle("Message from a customer!")
                .WithTimestamp(DateTime.Now));
                await Context.Message.DeleteAsync();
                await ReplyAsync(":thumbsup:");
                SS.LogCommand(Context, "Respond", new string[] { message});
            }
            else
            {
                await ReplyAsync("Your message must be longer then 5 characters.");
            }
        }


        [Command("orderinfo")]
        [Alias("oi")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task OrderInfo(int id)
        {
            if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
            {
                Sandwich order = SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value;
                Color c = new Color(102, 102, 153);
                await Context.Message.DeleteAsync();
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
                    if (order.OrderChef != null)
                    {
                        builder.Value = order.OrderChef.ChefName;
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
                .WithUrl("https://discord.gg/XgeZfE2")
                .WithColor(c)
                .WithThumbnailUrl(order.AvatarUrl)
                .WithTitle("Order information")
                .WithTimestamp(DateTime.Now));

                SS.LogCommand(Context, "Order Info", new string[] {id.ToString()});

                SS.Save();
            }
        }

        [Command("order")]
        [Alias("o")]
        [NotBlacklisted]
        [Summary("Ogre!")]
        [RequireBotPermission(GuildPermission.CreateInstantInvite)]
        public async Task Order([Remainder]string order)
        {
            
            if (order != null)
            {
                var i = 0;

                if (SS.hasAnOrder.ContainsKey(Context.User.Id)) { await ReplyAsync($"You already haven an order placed! :angry: "); return; }

                try
                {
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
                    ITextChannel usrclog = await usr.GetTextChannelAsync(SS.usrlogcID);

                    var r = new Random();
                    i = r.Next(1, 999);

                    while (SS.activeOrders.ContainsKey(i))
                    {
                        i = r.Next(1, 999);
                        await usrclog.SendMessageAsync($"<@131182268021604352> Rerolled order, matching ids. {i}"); //This line has never been used in the log channel. Sad!
                    }

                    var s = Context.Guild;
                    IGuildUser customer = await Context.Guild.GetUserAsync(Context.User.Id);


                    var o = new Sandwich(order, i, DateTime.Now, OrderStatus.Waiting, customer.GetAvatarUrl(), customer.Discriminator, customer.Username, customer.Guild.IconUrl, customer.Guild.Name, customer.Guild.DefaultChannelId, Context.Channel.Id, customer.Id, customer.Guild.Id);

                    SS.activeOrders.Add(i, o);
                    var builder = new EmbedBuilder();
                    builder.ThumbnailUrl = Context.User.GetAvatarUrl();
                    builder.Title = $" New order from {Context.Guild.Name}(`{Context.Guild.Id}`)";
                    var desc = $"Ordered by: **{Context.User.Username}**#**{Context.User.Discriminator}**(`{Context.User.Id}`)\n" +
                       $"Channel: `{Context.Channel.Name}`\n" +
                       $"Id: `{i}`\n" +
                       $"```{order}```";
                    builder.Description = desc;
                    builder.Color = new Color(84, 176, 242);
                    builder.WithFooter(x =>
                    {
                        x.Text = "Is this order abusive? Please tell Lemon or Fires immediately!";
                    });
                    builder.Timestamp = DateTime.Now;


                    SS.hasAnOrder.Add(Context.User.Id, i);
                    SS.totalOrders += 1;
                    var roles = Context.Guild.Roles;
                    var artist = roles.FirstOrDefault(x => x.Name == "Sandwich Artists");
                    await usrc.SendMessageAsync($"{artist.Mention}", embed: builder); //mention in the MESSAGE not the embed. smh
                    SS.LogCommand(Context, "Order", new string[] { order });



                }
                catch (Exception e)
                {
                    await ReplyAsync("This error should not happen! Contact Fires#1043 immediately!");
                    await ReplyAsync($"```{e}```");
                    Console.WriteLine(e);
                    return;
                }
                IDMChannel dm = await Context.User.CreateDMChannelAsync();
                IUserMessage m = await ReplyAsync(":thumbsup:");
                try
                {
                    await dm.SendMessageAsync($"Your order has been delivered! Thank you for ordering! Please wait while someone accepts your order. :slight_smile: - ID `{i}`");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await m.ModifyAsync(msg =>
                    {
                        msg.Content = $":thumbsdown: {Context.User.Mention} We failed to dm you. You're order has been automatically deleted. Please enable DMs and re order. http://i.imgur.com/vY7tThf.png OR http://i.imgur.com/EtaA78Q.png";
                    });
                    SS.totalOrders -= 1;
                    SS.hasAnOrder.Remove(Context.User.Id);
                    SS.activeOrders.Remove(i);
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
                    ITextChannel usrclog = await usr.GetTextChannelAsync(SS.usrlogcID);
                    await usrc.SendMessageAsync($"**IGNORE ORDER {i} AS IT HAS BEEN REMOVED**");
                    await usrclog.SendMessageAsync($"Order {i} has been removed due to the customer having their dms closed.");
                }

                SS.Save();
            }
            else { await ReplyAsync("You need to specify what you want idiot! :confused:"); }
        }

        [Command("acceptorder")]
        [Alias("ao")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task AcceptOrder(int id)
        {
            if (id > 0)
            {

                Chef c = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
                {
                    Sandwich order = SS.activeOrders.FirstOrDefault(a => a.Value.Id == id).Value;
                    if (SS.toBeDelivered.Contains(order.Id)) { await ReplyAsync("This order is already ready to be delivered! :angry: "); return; }
                    try //TODO: FIX 403 ERROR
                    {
                        await Context.Message.DeleteAsync();
                        await ReplyAsync($"{Context.User.Mention} Sandwich order is now ready for delivery! Please assemble the sandwich, once you are complete. `;deliver {id}` to continue! Type `;orderinfo {id}`(short: `;oi {id}`) if you need more info. :wave: ");
                        IGuild s = await Context.Client.GetGuildAsync(order.GuildId);
                        ITextChannel ch = await s.GetTextChannelAsync(order.ChannelId);
                        IGuildUser u = await s.GetUserAsync(order.UserId);
                        try
                        {
                            IDMChannel dm = await u.CreateDMChannelAsync();
                            var builder = new EmbedBuilder();

                            builder.ThumbnailUrl = Context.User.GetAvatarUrl();
                            builder.Title = $"Your order has been accepted by {Context.User.Username}#{Context.User.Discriminator}!";
                            var desc = $"```{order.Desc}```\n" +
                                       $"Id: `{order.Id}`\n" +
                                       $"**Watch this chat for an updates on when it is on it's way! It is ready for delivery!";
                            builder.Description = desc;
                            builder.Color = new Color(36, 78, 145);
                            builder.Url = "https://discord.gg/XgeZfE2";
                            builder.WithFooter(x =>
                            {
                                x.IconUrl = u.GetAvatarUrl();
                                x.Text = $"Ordered at: {order.date}.";
                            });
                            builder.Timestamp = DateTime.UtcNow;
                            order.Status = OrderStatus.ReadyToDeliver; //like a dirty jew
                            SS.toBeDelivered.Add(order.Id);
                            c.ordersAccepted += 1;
                            await dm.SendMessageAsync("", embed: builder);
                            SS.LogCommand(Context, "Accept Order", new string[] { id.ToString() });
                            SS.Save();
                        }
                        catch (NullReferenceException e)
                        {
                            // just silently fail for now, it's handled later.
                            //await ReplyAsync("Null ref. Did they kick our bot or delete the channel? Try to add the user and ask.");
                            //delete it too???
                            Console.WriteLine(e); //Better idea.
                        }
                        catch (Exception e)
                        {
                            await ReplyAsync("Error :ghost:");
                            await ReplyAsync($"```{e}```");
                            Console.WriteLine(e);
                            await ch.SendMessageAsync(u.Mention + " I cannot send dms to you! Please give me the ability to by going to the servers settings in the top left > privacy settings and enabled direct messages from server users. Thank you. If you believe this error was a mistake, please join our server using `;server` and contact Fires#1043.");
                            return;
                        }
                    }
                    catch (Exception d)
                    {
                        await ReplyAsync("SEND ERROR TO Fires#4553 IMMEDIATELY");
                        await ReplyAsync($"```{d}```");
                    }
                    SS.Save(); return;
                }
                else
                {
                    await ReplyAsync("Sorry bud this order doesn't exist!"); return;
                }
            }

        }



        [Command("deliver")]
        [Alias("d")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task Deliver(int id)
        {
            if (id > 0)
            {
                if (SS.toBeDelivered.Contains(id))
                {

                    if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
                    {
                        Chef c = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                        Sandwich order = SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value;
                        if (order.Status == OrderStatus.ReadyToDeliver)
                        {
                            try
                            {
                                Console.WriteLine("passed finish");
                                await Context.Message.DeleteAsync();
                                await ReplyAsync($"{Context.User.Mention} DMing you an invite! Go deliver it! Remember to be nice and ask for `;feedback`");
                                IGuild s = await Context.Client.GetGuildAsync(order.GuildId);
                                ITextChannel ch = await s.GetTextChannelAsync(order.ChannelId);
                                IGuildUser u = await s.GetUserAsync(order.UserId);
                                IDMChannel dm = await u.CreateDMChannelAsync();
                                await dm.SendMessageAsync($"Your sandwich is being delivered soon! Watch out!");
                                IInvite inv = await ch.CreateInviteAsync(0, 1, false, true);
                                IDMChannel artistdm = await Context.User.CreateDMChannelAsync();

                                var builder = new EmbedBuilder();
                                builder.ThumbnailUrl = order.AvatarUrl;
                                builder.Title = $"Your order is being delivered by {Context.User.Username}#{Context.User.Discriminator}!";
                                var desc = $"```{order.Desc}```\n" +
                                           $"**Incoming sandwich! Watch {order.GuildName}!**";
                                builder.Description = desc;
                                builder.Color = new Color(163, 198, 255);
                                builder.WithFooter(x =>
                                {
                                    x.IconUrl = u.GetAvatarUrl();
                                    x.Text = $"Ordered at: {order.date}.";
                                });
                                builder.Timestamp = DateTime.UtcNow;
                                await artistdm.SendMessageAsync("Invite:" + inv.ToString());
                                SS.LogCommand(Context, "Deliver", new string[] { id.ToString() });
                                SS.cache.Add(order);
                                order.Status = OrderStatus.Delivered;
                                SS.toBeDelivered.Remove(order.Id);
                                SS.activeOrders.Remove(order.Id);
                                SS.hasAnOrder.Remove(order.UserId);
                                c.ordersDelivered += 1;
                                //await e.Channel.SendMessage("The Order has been completed and removed from the system. You cannot go back now!");
                                SS.Save();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                if (ex.InnerException is NullReferenceException)
                                {

                                    await ReplyAsync("Order is corrupt. This usually means that the bot was removed or the channel was removed.");
                                } else {
                                    await ReplyAsync(":ghost:");
                                    //Console.WriteLine(ex);
                                    await ReplyAsync($"```{ex}```"); return;
                                }
                            }
                        }
                        else
                        {
                            await ReplyAsync($"This order is not ready to be delivered just yet! It is currently Order Status {order.Status}"); return;
                        }
                    }
                    else
                    {
                        await ReplyAsync("Invalid order probably (tell Fires its a problem with the checky thingy, the thing thing)"); return;
                    }
                }
                else
                {
                    await ReplyAsync("This order is not ready to be delivered yet! (this error can also occur if you are not using the right id)"); return;
                }
            }
        }


        [Command("denyorder")]
        [Alias("do")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task DenyOrder(int id, [Remainder] string reason)
        {

            if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
            {
                Sandwich order = SS.activeOrders.FirstOrDefault(a => a.Value.Id == id).Value;
                order.Status = OrderStatus.Delivered;
                SS.activeOrders.Remove(id);
                await ReplyAsync($"{Context.User.Mention} Deleted order {order.Id}!");
                IGuild s = await Context.Client.GetGuildAsync(order.GuildId);
                ITextChannel ch = await s.GetTextChannelAsync(order.ChannelId);
                IGuildUser u = await s.GetUserAsync(order.UserId);
                IDMChannel dm = await u.CreateDMChannelAsync();
                await Context.Message.DeleteAsync();
                SS.LogCommand(Context, "Deny Order", new string[] { id.ToString() });
                SS.hasAnOrder.Remove(order.UserId);
                SS.totalOrders -= 1;
                await dm.SendMessageAsync($"Your sandwich order has been denied! ", embed: new EmbedBuilder()
                    .WithThumbnailUrl(Context.User.GetAvatarUrl())
                    .WithUrl("https://discord.gg/XgeZfE2")
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
            else
            {
                await ReplyAsync("This order does not exist!"); return;
            }
        }


        [Command("delorder")]
        [Alias("delo")]
        [NotBlacklisted]
        public async Task DelOrder()
        {
            try
            {
                Sandwich order = SS.activeOrders.First(s => s.Value.UserId == Context.User.Id).Value;
                IUserMessage msg = await ReplyAsync("Deleting order...");
                SS.activeOrders.Remove(order.Id);
                SS.totalOrders -= 1;
                SS.hasAnOrder.Remove(order.UserId);
                IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
                await usrc.SendMessageAsync($"Order `{order.Id}`,`{order.Desc}` has been **REMOVED**.");
                SS.LogCommand(Context, "Delete Order");
                SS.Save();
                await msg.ModifyAsync(x =>
                {
                    x.Content = "Successfully deleted order!";
                });
            }
            catch (Exception e)
            {
                await ReplyAsync("Failed to delete. Are you sure you have one?");
                Console.WriteLine(e);
            }
        }

        [Command("caniblacklist")]
        [Alias("cib")]
        [NotBlacklisted]
        public async Task CanIBlacklist()
        {
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    await ReplyAsync("**Yes.**");
                }
                else
                {
                    await ReplyAsync("**No.**");
                }
            }
            else
            {
                await ReplyAsync("You are not a Sandwich Artist!");
            }
            SS.LogCommand(Context, "Can I blacklist?");
        }

        [Command("feedback")]
        [Alias("f")]
        [NotBlacklisted]
        public async Task Feedback([Remainder]string f)
        {
            if (SS.givenFeedback.Contains(Context.User.Id)) { await ReplyAsync("You've already sent feedback :)"); return; }

            if (f != null)
            {
                try
                {
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(306941357795311617);

                    var builder = new EmbedBuilder();
                    builder.ThumbnailUrl = Context.User.GetAvatarUrl();
                    builder.Title = $"New feedback from {Context.User.Username}#{Context.User.Discriminator}(`{Context.User.Id}`)";
                    var desc = $"{f}";
                    builder.Description = desc;
                    builder.Color = new Color(242, 255, 5);
                    builder.WithFooter(x =>
                    {
                        x.Text = "Is this feedback abusive? Please tell Lemon, Fires or Beymoezy immediately!";
                    });
                    builder.Timestamp = DateTime.Now;

                    await usrc.SendMessageAsync("", embed: builder);
                    await ReplyAsync("Thank you!");
                    SS.givenFeedback.Add(Context.User.Id);
                    SS.LogCommand(Context, "Feedback", new string[] { f });
                    SS.Save();
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

        [Command("server")]
        [Alias("serv", "s")]
        [NotBlacklisted]
        public async Task servercom()
        {
            if (SS.blacklisted.Contains(Context.User.Id) || SS.blacklisted.Contains(Context.Guild.Id)) { await Context.Channel.SendMessageAsync("You have been blacklisted from this bot. :cry: "); return; }
            await ReplyAsync("Come join our server! Feel free to shitpost, spam and do whatever! https://discord.gg/XgeZfE2");
            SS.LogCommand(Context, "Server");
        }

        [Command("motd")]
        [NotBlacklisted]
        public async Task MOTD()
        {
            await ReplyAsync(SS.motd);
            SS.LogCommand(Context, "MOTD");
        }

        [Command("blacklist")]
        [Alias("b")]
        [RequireBlacklist]
        public async Task Blacklist(ulong id)
        {
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    SS.blacklisted.Add(id);
                    await ReplyAsync("Successfully blacklisted! :thumbsup: ");
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrlogcID);
                    await usrc.SendMessageAsync($"{Context.User.Mention} blacklisted <@{id}>(id).");
                    SS.LogCommand(Context, "Blacklist", new string[] { id.ToString() });
                    SS.Save();
                }
                else
                {
                    await ReplyAsync("You cannot do this!");
                }
            }
            else
            {
                await ReplyAsync("No can do!");
            }
        }
        [Command("blacklist")]
        [Alias("b")]
        [RequireBlacklist]
        public async Task Blacklist(IGuildUser user)
        {
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    SS.blacklisted.Add(user.Id);
                    await ReplyAsync("Successfully blacklisted! :thumbsup: ");
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrlogcID);
                    await usrc.SendMessageAsync($"{Context.User.Mention} blacklisted <@{user.Id}>(user).");
                    SS.LogCommand(Context, "Blacklist", new string[] { user.Id.ToString() });
                    SS.Save();
                }
                else
                {
                    await ReplyAsync("You cannot do this!");
                }
            }
            else
            {
                await ReplyAsync("No can do!");
            }
        }

        [Command("unblacklist")]
        [Alias("ub")]
        [RequireBlacklist]
        public async Task removeFromBlacklist(ulong id)
        {
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    SS.blacklisted.Remove(id);
                    await ReplyAsync("Removed! :thumbsup: ");
                    SS.LogCommand(Context, "Remove From Blacklist", new string[] { id.ToString() });
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrlogcID);
                    await usrc.SendMessageAsync($"{Context.User.Mention} unblacklisted <@{id}>(id).");
                    SS.Save();
                }
                else
                { await ReplyAsync("You cannot do this!"); }
            }
            else
            {
                await ReplyAsync("You are not an Artist!");
            }
        }
        [Command("unblacklist")]
        [Alias("ub")]
        [RequireBlacklist]
        public async Task removeFromBlacklist(IGuildUser user)
        {
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    SS.blacklisted.Remove(user.Id);
                    await ReplyAsync("Removed! :thumbsup: ");
                    SS.LogCommand(Context, "Remove From Blacklist", new string[] { user.Id.ToString() });
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrlogcID);
                    await usrc.SendMessageAsync($"{Context.User.Mention} unblacklisted <@{user.Id}>(user).");
                    SS.Save();
                }
                else
                { await ReplyAsync("You cannot do this!"); }
            }
            else
            {
                await ReplyAsync("You are not an Artist!");
            }
        }



        //[Command("blacklistuser")]
        //[Alias("bu")]
        //[RequireBlacklist]
        //public async Task BlacklistUser(IGuildUser user)
        //{
        //    if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
        //    {
        //        Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
        //        if (s.canBlacklist)
        //        {
        //            SS.blacklisted.Add(user.Id);
        //            await ReplyAsync("Successfully blacklisted! :thumbsup: ");
        //            IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
        //            ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrlogcID);
        //            await usrc.SendMessageAsync($"{Context.User.Mention} blacklisted user {user.Mention}.");
        //            SS.LogCommand(Context, "Blacklist User", new string[] { user.Username });
        //            SS.Save();
        //        }
        //        else
        //        {
        //            await ReplyAsync("You cannot do this!");
        //        }
        //    }
        //    else
        //    {
        //        await ReplyAsync("No can do!");
        //    }
        //}

        //[Command("removefromblacklist")]
        //[Alias("rfb")]
        //[RequireBlacklist]
        //public async Task removeFromBlacklist(ulong id)
        //{
        //    if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
        //    {
        //        Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
        //        if (s.canBlacklist)
        //        {
        //            SS.blacklisted.Remove(id);
        //            await ReplyAsync("Removed! :thumbsup: ");
        //            SS.LogCommand(Context, "Remove From Blacklist", new string[] { id.ToString() });
        //            SS.Save();
        //        }
        //        else
        //        { await ReplyAsync("You cannot do this!"); }
        //    }
        //    else
        //    {
        //        await ReplyAsync("You are not an Artist!");
        //    }
        //}

        //[Command("removeuserfromblacklist")]
        //[Alias("rufb")]
        //[inUSR]
        //[RequireBlacklist]
        //public async Task removeUserFromBlacklist(IGuildUser user)
        //{
        //    if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
        //    {
        //        Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
        //        if (s.canBlacklist)
        //        {
        //            SS.blacklisted.Remove(user.Id);
        //            await ReplyAsync("Removed! :thumbsup: ");
        //            SS.LogCommand(Context, "Remove User From Blacklist", new string[] { user.Username });
        //            SS.Save();
        //        }
        //        else
        //        { await ReplyAsync("You cannot do this!"); }
        //    }
        //    else
        //    {
        //        await ReplyAsync("You are not an Artist!");
        //    }
        //}

        [Command("totalorders")]
        [Alias("to")]
        [NotBlacklisted]
        public async Task TotalOrders()
        {
            await ReplyAsync($"We have proudly served {SS.totalOrders} sandwiches since April 26th 2017, We currently have {SS.cache.Count} saved.");
            SS.LogCommand(Context, "Total Orders");
        }

        [Command("credits")]
        [Alias("cred")]
        [NotBlacklisted]
        public async Task credits()
        {
            await ReplyAsync($"Special thanks to ``` \r\n Melon - no, you suck \r\n JeuxJeux20 - Json help, bot wouldn't exist without you.  \r\n LewisTehMinerz - Made Fires not be lonely in the project, and has done stuff for him. \r\n Bloxri - Assorted C# knowledge \r\n Discord Pizza - Inspiration \r\n Discord Api DiscordNet Channel Members - Helped me get Discord.Net 1.0 set up and working, Love you flam: kissing_heart: \r\n Aux - Evaluate command \r\n All of the folks from the USR discord. :)```");
            SS.LogCommand(Context, "Credits");
        }

        [Command("help")]
        [Alias("h")]
        public async Task Help()
        {
            await ReplyAsync(@"**__COMMANDS__**
        »order
            ; order medium blt with extra lettuce
            Orders something!
        »feedback
            ; feedback I didnt get as much extra lettuce as I would have liked, but it was enough. Thanks!
             ; feedback I didn't get anything close to my order! WTF
            Sends feedback back to our server, It it highly recommended to include the name of your delivery man / woman, but please do NOT @ THEM IN THE FEEDBACK
        »motd
            ; motd
             Send message that is sent when the bot first 
        »total orders
            ; totalorders
             Returns the amount of orders we have done!
        »credits
            ; credits
             Returns the credits
        »help
            ; help
             THIS
        »server
            ; server
             Gets our server!
         ");
            SS.LogCommand(Context, "Help");
        }
        
    }
}

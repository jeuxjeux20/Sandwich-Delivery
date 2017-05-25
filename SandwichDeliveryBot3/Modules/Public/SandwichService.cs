using System;
using System.Collections.Generic;
using System.Linq;
using SandwichBot.SandwichBase;
using Newtonsoft.Json;
using System.IO;
using Discord;
using Discord.WebSocket;
using SandwichBot.ChefBase;
using Discord.Commands;
using System.Threading.Tasks;

namespace Dopost.SandwichService
{
    public class SandwichService
    {
        public Dictionary<int, Sandwich> activeOrders = new Dictionary<int, Sandwich>();
        public Dictionary<ulong, int> hasAnOrder = new Dictionary<ulong, int>();
        public Dictionary<string, Chef> chefList = new Dictionary<string, Chef>();
        public static List<ulong> blacklisted = new List<ulong>();
        public List<ulong> givenFeedback = new List<ulong>();
        public List<int> toBeDelivered = new List<int>();
        public List<Sandwich> cache = new List<Sandwich>();
        public int totalOrders = 0;
        public string version = "2.3";
        public string date = "May 8th 2017, 5pm CST";
        public string updatename = "Added new method to main server called 'LogCommand', logs all activity to our 'commandlog' channel. Minor fixes and changes that can be viewed by the commit history over at the github.";
        public string motd;
        public ulong usrID = 264222431172886529;    //264222431172886529  297910882976006154
        public ulong usrcID = 285529162511286282;   //285529162511286282 298552977504075777
        public ulong usrlogcID = 287990510428225537; //287990510428225537 306909741622362112

       

        public void Save()
        {
            try
            {
                using (var sw = new StreamWriter(@"data/orders.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, activeOrders);
                    Console.WriteLine("serialized order");
                }
                using (var sw = new StreamWriter(@"data/ordercount.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, totalOrders);
                    Console.WriteLine("serialized order count");
                }
                using (var sw = new StreamWriter(@"data/cache.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, cache);
                    Console.WriteLine("serialized cache");
                }
                using (var sw = new StreamWriter(@"data/blacklisted.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, blacklisted);
                    Console.WriteLine("serialized blacklist");
                }
                using (var sw = new StreamWriter(@"data/givenfeedback.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, givenFeedback);
                    Console.WriteLine("serialized feedback list");
                }
                using (var sw = new StreamWriter(@"data/chef.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, chefList);
                    Console.WriteLine("serialized chef");
                }
            }
            catch (Exception n)
            {
                Console.WriteLine("Failed to save!");
                Console.WriteLine(n);
            }
        }

        public async void LogCommand(ICommandContext context, string commandname, string[] arguments = null) //Pass all of our variables into the command
        //example: LogCommand(client, Context.User, Context.Guild, Context.Channel, "Order", new string[]{orderinfo, blah, blah}); should hopefully supply all info needed.
        {
            //Grab important channels
            IGuild usr = await context.Client.GetGuildAsync(usrID);
            ITextChannel clogchannel = await usr.GetTextChannelAsync(311240926615830529);
            //Create special variables
            try
            {
                arguments = arguments ?? new string[] { "None given." };
                var fullname = string.Format("{0}#{1}({2})", context.User.Username, context.User.Discriminator, context.User.Id);
                var fullservername = string.Format("{0}({1})", context.Guild.Name ?? "unable to get name", context.Guild.Id); //Crashes a LOT on this line
                var fullchannelname = string.Format("{0}({1})", context.Channel.Name, context.Channel.Id);
                var argumentparse = string.Join(" , ", arguments);
                var c = new Color(255, 169, 33);

                //Build and send embed
                await clogchannel.SendMessageAsync("Command Used:", embed: new EmbedBuilder()
                    .AddField(builder =>
                    {
                        builder.Name = "Command:";
                        builder.Value = commandname;
                        builder.IsInline = true;
                    })
                   .AddField(builder =>
                   {
                       builder.Name = "User:";
                       builder.Value = fullname;
                       builder.IsInline = true;
                   })
                   .AddField(builder =>
                   {
                       builder.Name = "Server:";
                       builder.Value = context.Guild.Name;
                       builder.IsInline = true;
                   })
                   .AddField(builder =>
                   {
                       builder.Name = "Channel:";
                       builder.Value = fullchannelname;
                       builder.IsInline = true;
                   })
                   .AddField(builder =>
                   {
                       builder.Name = "Arguments:";
                       builder.Value = argumentparse;
                       builder.IsInline = true;
                   })
                   .AddField(builder =>
                   {
                       builder.Name = "Date:";
                       builder.Value = DateTime.Now;
                       builder.IsInline = true;
                   })
                   .AddField(builder =>
                   {
                       builder.Name = "Raw Message:";
                       builder.Value = "```" + context.Message.Content + "```";
                       builder.IsInline = true;
                   })
                   .WithUrl("https://discord.gg/XgeZfE2")
                   .WithColor(c)
                   .WithThumbnailUrl(context.User.GetAvatarUrl())
                   .WithTitle($"New command used by `{context.User.Username}#{context.User.Discriminator}`, in `#{context.Channel.Name}` at `{context.Guild.Name}`.")
                   .WithTimestamp(DateTime.Now));
            }
            catch (Exception e)
            {
                await clogchannel.SendMessageAsync($"<@131182268021604352> ay fam critical error. ```{e}```"); //hope this fixes. defensive programming amirite
            }
        }

        public void Load()
        {
            try
            {
                using (var sr = new StreamReader(@"data/orders.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    activeOrders = JsonSerializer.Create().Deserialize<Dictionary<int, Sandwich>>(myLovelyReader);
                    Console.WriteLine("Deserialized Orders.");
                    Console.WriteLine(activeOrders.Count());
                }
                using (var sr = new StreamReader(@"data/blacklisted.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    blacklisted = JsonSerializer.Create().Deserialize<List<ulong>>(myLovelyReader);
                    Console.WriteLine("Deserialized Blacklist.");
                    Console.WriteLine(blacklisted.Count());
                }
                using (var sr = new StreamReader(@"data/givenfeedback.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    givenFeedback = JsonSerializer.Create().Deserialize<List<ulong>>(myLovelyReader);
                    Console.WriteLine("Deserialized GivenFeedback.");
                    Console.WriteLine(blacklisted.Count());
                }
                using (var sr = new StreamReader(@"data/ordercount.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    totalOrders = JsonSerializer.Create().Deserialize<int>(myLovelyReader);
                    Console.WriteLine("Deserialized number of total orders.");
                    Console.WriteLine(totalOrders);
                }
                using (var sr = new StreamReader(@"data/cache.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    cache = JsonSerializer.Create().Deserialize<List<Sandwich>>(myLovelyReader);
                    Console.WriteLine("Deserialized order cache.");
                    Console.WriteLine(cache.Count());
                }
                using (var sr = new StreamReader(@"data/chef.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    chefList = JsonSerializer.Create().Deserialize<Dictionary<string, Chef>>(myLovelyReader);
                    Console.WriteLine("Deserialized chef.");
                    Console.WriteLine(chefList.Count());
                }
                using (var sr = new StreamReader(@"data/motd.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    motd = JsonSerializer.Create().Deserialize<string>(myLovelyReader);
                    Console.WriteLine("Deserialized motd.");
                    Console.WriteLine(motd);
                }
            }
            catch
            {
                Console.WriteLine("ERROR TERROR MY FRIENDO | BOY LOTS OF SHIT IS GOING WRONG NOW. THE FILE IS BROKE FAM");
            }
        }
    }

}
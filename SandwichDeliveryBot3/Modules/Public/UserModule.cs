using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot3.Precons;
using Microsoft.Extensions.DependencyInjection;
using SandwichDeliveryBot.Databases;
using SandwichDeliveryBot3.CustomClasses;
using SandwichDeliveryBot3.Enums;

namespace SandwichDeliveryBot.UtilityMod
{
    public class UserModule : ModuleBase
    {
        UserDatabase _udb;

        public UserModule(IServiceProvider provider)
        {
            _udb = provider.GetService<UserDatabase>();
        }
        [Command("toggleusertype")]
        [RequireBlacklist]
        public async Task changeOrders(IGuildUser user)
        {
            await _udb.ToggleUserType(user.Id);
        }
        [Command("changeorders")]
        [RequireBlacklist]
        public async Task changeOrders(IGuildUser user, int diff)
        {
            await _udb.ChangeOrders(user.Id, diff);
        }
        [Command("changedenials")]
        [RequireBlacklist]
        public async Task changeDenials(IGuildUser user, int diff)
        {
            await _udb.ChangeDenials(user.Id, diff);
        }
        [Command("changerank")]
        [RequireBlacklist]
        public async Task changeRank(IGuildUser user, UserRank rank)
        {
            await _udb.ChangeRank(user.Id, rank);
        }

        [Command("userinfo")]
        public async Task Invite(IGuildUser user = null)
        {
            SandwichUser u;
            if (user == null)
                u = await _udb.FindUser(Context.User.Id);
            else
                u = await _udb.FindUser(user.Id);

            Color c;
            if(u.IsBlacklisted)
                 c = new Color(198, 0, 53);
            else
                c = new Color(54, 219, 148);
       
            await ReplyAsync($"{Context.User.Mention} Here is your requested information!", embed: new EmbedBuilder()
            .AddField(builder =>
            {
                builder.Name = "**User**";
                builder.Value = u.Name+"#"+u.Distin;
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "Is Blacklisted?";
                builder.Value = u.IsBlacklisted;
                builder.IsInline = true;
            })
             .AddField(builder =>
             {
                 builder.Name = "Tips";
                 builder.Value = u.Tips;
                 builder.IsInline = true;
             })
            .AddField(builder =>
            {
                builder.Name = "Rank";
                var val = u.Rank.ToString();
                builder.Value = string.Concat(val.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' '); 
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "User Type";
                builder.Value = u.Type;
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "Orders";
                builder.Value = u.Orders;
                builder.IsInline = true;
            })
             .AddField(builder =>
             {
                 builder.Name = "Denied Orders";
                 builder.Value = u.Denials;
                 builder.IsInline = true;
             })
            .WithUrl(new Uri("https://discord.gg/XgeZfE2"))
            .WithColor(c)
            .WithTitle("User information")
            .WithTimestamp(DateTime.Now));
        }
    }
}
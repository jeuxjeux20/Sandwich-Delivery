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

namespace SandwichDeliveryBot.UtilityMod
{
    public class TipModule : ModuleBase
    {
        SandwichService _SS;
        SandwichDatabase _DB;
        ArtistDatabase _ADB;
        ListingDatabase _LDB;
        UserDatabase _UDB;
        TipDatabase _TDB;

        public TipModule(IServiceProvider provider)
        {
            _SS = provider.GetService<SandwichService>();
            _DB = provider.GetService<SandwichDatabase>();
            _ADB = provider.GetService<ArtistDatabase>();
            _LDB = provider.GetService<ListingDatabase>();
            _UDB = provider.GetService<UserDatabase>();
            _TDB = provider.GetService<TipDatabase>();
        }

        [Command("tipinfo")]
        public async Task TipInfo()
        {
            await ReplyAsync("You earn more tips by leveling up(check level with `;userinfo`). You can level up by ordering sandwiches. But please do not spam it. For more information on leveling up, join our `;server` and type `;tag ranks` in the main chat.");
        }

        [Command("tip")]
        public async Task Tip(IGuildUser user)
        {
            IUser userr = user as IUser;
            IGuild USR = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel log = await USR.GetTextChannelAsync(_SS.TipId);
            SandwichUser u = await _UDB.FindUser(Context.User.Id);
            if (u.Tips <= 0) { await ReplyAsync("You have no more tips left! You earn more tips by leveling up(check level with `;userinfo`)."); return; }
            if (userr == Context.User) { await ReplyAsync("You can't tip yourself. That is cheating!"); await log.SendMessageAsync($"**{Context.User.Username}#{Context.User.Discriminator}** just tried to tip themselves...");  return; }
            Artist rec = await _ADB.FindArtist(user.Id);
            if (rec == null) { await ReplyAsync("You can only tip Sandwich Artists."); return; }
            await _TDB.NewTip(Context.User.Username + "#" + Context.User.Discriminator, user.Username + "#" + user.Discriminator);
            await _UDB.ChangeTips(u, await _UDB.FindUser(user.Id));
            await log.SendMessageAsync($"**{Context.User.Username}#{Context.User.Discriminator}** has tipped **{user.Username}#{user.Discriminator}**.");
            await ReplyAsync($"Thank you for tipping, you now have **{u.Tips}** tips left. For information on earning tips, type `;tipinfo`.");
        }
    }
}
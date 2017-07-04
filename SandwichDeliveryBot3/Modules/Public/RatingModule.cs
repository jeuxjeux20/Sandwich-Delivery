//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Discord;
//using Discord.Commands;
//using Discord.WebSocket;
//using System.Runtime.InteropServices;
//using System.Diagnostics;
//using SandwichDeliveryBot.SService;
//using SandwichDeliveryBot3.Precons;
//using Microsoft.Extensions.DependencyInjection;
//using SandwichDeliveryBot.Databases;
//using SandwichDeliveryBot3.CustomClasses;

//namespace SandwichDeliveryBot.UtilityMod
//{
//    public class RatingModule : ModuleBase
//    {
//        SandwichService _SS;
//        SandwichDatabase _DB;
//        ArtistDatabase _ADB;
//        ListingDatabase _LDB;
//        UserDatabase _UDB;
//        TipDatabase _TDB;

//        public RatingModule(IServiceProvider provider)
//        {
//            _SS = provider.GetService<SandwichService>();
//            _DB = provider.GetService<SandwichDatabase>();
//            _ADB = provider.GetService<ArtistDatabase>();
//            _LDB = provider.GetService<ListingDatabase>();
//            _UDB = provider.GetService<UserDatabase>();
//            _TDB = provider.GetService<TipDatabase>();
//        }

//        [Command("rate")]
//        public async Task Rate(IGuildUser user, float r)
//        {
//            Artist a = await _ADB.FindArtist(user.Id);
//            IGuild USR = await Context.Client.GetGuildAsync(_SS.USRGuildId);
//            ITextChannel log = await USR.GetTextChannelAsync(_SS.LogId);
//            if (r >= 0.0f && r <= 5.0f)
//            {
//                int ratings = a.Ratings;
//                float currentrating = a.Rating;
//                float totalrating = currentrating * ratings;
//                totalrating += r;
//                float newrating = totalrating / ratings + 1;
//                double mult = Math.Pow(10.0, 2);
//                double result = Math.Truncate(mult * newrating) / mult;

//                a.Rating = (float)result;
//                a.Ratings += 1;

//                await _ADB.SaveChangesAsync();
//                await ReplyAsync($"Thank you for rating your Sandwich Artist, you are their {ratings+1}th/st/rd reviewer. You have moved their rating from {currentrating} to {newrating}.");
//                await log.SendMessageAsync($"**{Context.User.Username}#{Context.User.Discriminator}** has rated **{user.Username}#{user.Discriminator}** at **{r}**. This has moved their rating from {currentrating} to {newrating} with {ratings+1} total ratings.");
//            }
//            else
//            {
//                await ReplyAsync("Your rating can only be between 0.0 and 5.0.");
//            }
//        }
//        [Command("rate")]
//        public async Task Rate(string user, float r)
//        {
//            Artist a = await _ADB.FindArtist(user);
//            if (a != null)
//            {
//                IGuild USR = await Context.Client.GetGuildAsync(_SS.USRGuildId);
//                ITextChannel log = await USR.GetTextChannelAsync(_SS.LogId);
//                if (r >= 0.0f && r <= 5.0f)
//                {
//                    int ratings = a.Ratings;
//                    float currentrating = a.Rating;
//                    float totalrating = currentrating * ratings;
//                    totalrating += r;
//                    float newrating = totalrating / ratings + 1;
//                    await ReplyAsync($"Thank you for rating your Sandwich Artist, you are their {ratings + 1}th/st/rd reviewer. You have moved their rating from {currentrating} to {newrating}.");
//                    await log.SendMessageAsync($"**{Context.User.Username}#{Context.User.Discriminator}** has rated **{a.ArtistName}#{a.ArtistDistin}** at **{r}**. This has moved their rating from {currentrating} to {newrating} with {ratings + 1} total ratings.");
//                }
//                else
//                {
//                    await ReplyAsync("Your rating can only be between 0.0 and 5.0.");
//                }
//            }
//            else
//            {
//                await ReplyAsync("Unable to locate this user in the database. Make sure you are including their discriminator too, like `Example#1234`.");
//            }
//        }
//    }
//}
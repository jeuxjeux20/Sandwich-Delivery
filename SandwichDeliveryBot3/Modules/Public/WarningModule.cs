//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Discord;
//using Discord.Commands;
//using SandwichDeliveryBot.WarningDB;
//using SandwichDeliveryBot.SService;
//using SandwichDeliveryBot.WarningClass;


//namespace SandwichDeliveryBot.WarningMod
//{
//    [Group("warnings"), Name("Warnings")]
//    public class WarningModule : ModuleBase
//    {
//         WarningDatabase _db;
//         SandwichService _ss;

//        public WarningModule(WarningDatabase db, SandwichService ss)
//        {
//            _db = db;
//            _ss = ss;
//        }

//        [Command, Priority(0)]
//        public async Task WarningsAsync()
//        {
//            var warn = await _db.GetWarnings();

//            if (warn == null)
//            {
//                throw new NoWarningsException("No warnings exist...at all, db fuck up???");
//            }
//            string result;
//            if (warn.Length > 0)
//            {
//                result = string.Join(", \r\n", warn.Select(x => string.Format("{0}   :   {1}", x.UserName, x.Reason)).ToArray());
//            }
//            else
//            {
//                result = "None in database.";
//            }

//                await ReplyAsync($"{result}");
//        }

//        [Command("create"), Priority(10)]
//        public async Task NewWarning(ulong Id, ulong Guildid, string Name, [Remainder] string r) {
//            await _db.CreateNewWarning( r, Id, Guildid, Name);
//            await ReplyAsync("Done!");
//        }

//        [Command("searchuser"), Priority(10)] //Worst method ever. For fucks sake
//        public async Task SearchUser(IGuildUser u)
//        {
//            var warnings = await _db.GetWarningsOnUser(u);
//            string result = "Nothing for result";
//            string idresult = "Nothing for id";
//            string nameresult = "Nothing for name";
//            if (warnings != null)
//            {
//                result = string.Join(", \r\n", warnings.Select(x => string.Format("{0}   :   {1}", x.UserName, x.Reason)).ToArray());
//                await ReplyAsync("Warnings were found on this users name: \r\n" + result);
//                return;
//            }
//            else
//            {
//                var idwarnings = await _db.GetWarningsOnUserId(u.Id);
//                if (idwarnings != null)
//                {
//                    idresult = string.Join(", \r\n", idwarnings.Select(x => string.Format("{0}   :   {1}", x.UserName, x.Reason)).ToArray()); await ReplyAsync("Warnings were found on this users id: \r\n" + idresult); return;
//                }

//                var namewarnings = await _db.GetWarningsOnUser(u.Username+"#"+u.Discriminator);
//                if (namewarnings != null)
//                {
//                    nameresult = string.Join(", \r\n", idwarnings.Select(x => string.Format("{0}   :   {1}", x.UserName, x.Reason)).ToArray()); await ReplyAsync("Warnings were found on this users name (2): \r\n" + nameresult); return;
//                }
//                if (idwarnings == null && namewarnings == null && warnings == null)
//                {
//                    await ReplyAsync("No warnings were found on this user.");
//                }
//            }

//        }

//        [Command("examplewarning"), Priority(10)]
//        public async Task ExampleWarning()
//        {
//            await ReplyAsync(";warnings create 131182268021604352 264222431172886529 Fires#1043 Reason: Bad guy \r\n ;warnings create UserID GuildID USERWITH#DISCRIMINATOR Reason because yeah yeah yeah");
//        }

//        [Command("delete"), Priority(10)]
//        public async Task DeleteWarning([Remainder] string Name)
//        {
//            await _db.DeleteWarning(Name);
//            await ReplyAsync("Done!");
//        }
//    }
//}

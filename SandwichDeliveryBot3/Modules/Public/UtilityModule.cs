using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SandwichBot.SandwichBase;
using Dopost.SandwichService;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Reflection;
using Microsoft.CodeAnalysis.Scripting;
using SandwichBot.ChefBase;

namespace UtilityModuleNameSpace
{
    public class UtiltyModule : ModuleBase
    {
        List<ulong> blacklisted = SandwichService.blacklisted;
        SandwichService SS;
        public UtiltyModule(SandwichService s)
        {
            SS = s;
        }

        [Command("invite")]
        [Summary("Returns the OAuth2 Invite URL of the bot")]
        public async Task Invite()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            await ReplyAsync(
                $"A user with `MANAGE_SERVER` can invite me to your server here: <https://discordapp.com/oauth2/authorize?client_id={application.Id}&scope=bot>");
            SS.LogCommand(Context, "Invite");
        }

        [Command("updateinfo")]
        public async Task Update()
        {
            await ReplyAsync(SS.version);
            await ReplyAsync(SS.updatename);
            await ReplyAsync(SS.date);
            SS.LogCommand(Context, "Update Info");
        }

        [Command("info")]
        public async Task Info()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            await ReplyAsync(
                $"{Format.Bold("Info")}\n" +
                $"- Author: {application.Owner.Username} (ID {application.Owner.Id})\n" +
                $"- Library: Discord.Net ({DiscordConfig.Version})\n" +
                $"- Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}\n" +
                $"- Uptime: {GetUptime()}\n\n" +
                $"- Current Order Count: {SS.activeOrders.Count}\n" +
                $"- ToBeDelivered Count: {SS.toBeDelivered.Count}\n" +
                $"- HasAnOrder Count: {SS.hasAnOrder.Count}\n" +
                $"- GivenFeedback Count: {SS.givenFeedback.Count}\n" +
                $"- ChefList Count: {SS.chefList.Count}\n" +
                $"- Blacklisted Count: {blacklisted.Count}\n" +
                $"- Cache Count: {SS.cache.Count}\n" +
                $"- Total Order Count: {SS.totalOrders}\n" +
                $"- Bot Version: {SS.version}\n" +
                $"- Update: {SS.updatename}\n\n" +
                $"- Update Date: {SS.date}\n" +

                $"{Format.Bold("Stats")}\n" +
                $"- Heap Size: {GetHeapSize()} MB\n" +
                $"- Guilds: {(Context.Client as DiscordSocketClient).Guilds.Count}\n" +
                $"- Channels: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count)}" +
                $"- Users: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count)}"
            );
            SS.LogCommand(Context, "Info");
        }

        [Command("eval")]
        public async Task Evaluate([Remainder]string code)
        {
            if (Context.User.Id == 131182268021604352)
            {
                var _timer = new Stopwatch();
                _timer.Start();
                var clean = GetFormattedCode("cs", code);
                var options = GetOptions();


                object r;
                try
                {
                    r = await CSharpScript.EvaluateAsync(code, options, new SandwichService());
                }
                catch (Exception e)
                {
                    r = e;
                }
                _timer.Stop();
                Embed n = GetEmbed(code, r, _timer.ElapsedMilliseconds);
                await ReplyAsync("", embed: n);
            }
            else
            {
                await ReplyAsync("You do not have permission to use this command!");
            }
        }


        public ScriptOptions GetOptions()
        {
            var options = ScriptOptions.Default
            .AddReferences(
                typeof(string).GetTypeInfo().Assembly,
                typeof(Assembly).GetTypeInfo().Assembly,
                typeof(Task).GetTypeInfo().Assembly,
                typeof(Enumerable).GetTypeInfo().Assembly,
                typeof(List<>).GetTypeInfo().Assembly,
                typeof(IGuild).GetTypeInfo().Assembly,
                typeof(SocketGuild).GetTypeInfo().Assembly,
                typeof(Chef).GetTypeInfo().Assembly,
                typeof(Sandwich).GetTypeInfo().Assembly
            )
            .AddImports(
                "System",
                "System.Reflection",
                "System.Threading.Tasks",
                "System.Linq",
                "System.Collections.Generic",
                "Discord",
                "Discord.WebSocket",
                "SandwichBot.ChefBase",
                "SandwichBot.SandwichBase",
                "SandwichBot",
                "Discord.Commands",
                "ChefStatusEnums",
                "OrderStatusEnums",
                "Dopost.SandwichService"
            );

            return options;
        }

        public string GetFormattedCode(string language, string rawmsg)
        {
            string code = rawmsg;

            if (code.StartsWith("```"))
                code = code.Substring(3, code.Length - 6);
            if (code.StartsWith(language))
                code = code.Substring(2, code.Length - 2);

            code = code.Trim();
            code = code.Replace(";\n", ";");
            code = code.Replace("; ", ";");
            code = code.Replace(";", ";\n");

            return code;
        }


        public Embed GetEmbed(string code, object result, long executeTime)
        {
            var builder = new EmbedBuilder();
            builder.Color = new Color(25, 128, 0);
            builder.AddField(x =>
            {
                x.Name = "Code";
                x.Value = $"```cs\n{code}```";
            });
            builder.AddField(x =>
            {
                x.Name = $"Result<{result?.GetType().FullName ?? "null"}>";

                if (result is Exception ex)
                    x.Value = ex.Message;
                else
                    x.Value = result ?? "null";
            });
            builder.WithFooter(x =>
            {
                x.Text = $"In {executeTime}ms";
            });

            return builder;
        }
    

    //public async Task<Embed> EvalAsync(SocketCommandContext context, string content)
    //{
    //    var _timer = new Stopwatch();
    //    _timer.Start();

    //    var cleancode = GetFormattedCode("cs", content);
    //    var options = GetOptions();
    //    object result;

    //    try
    //    {
    //        result = await CSharpScript.EvaluateAsync(cleancode, options, new RoslynGlobals(_provider, context));
    //    }
    //    catch (Exception ex)
    //    {
    //        result = ex;
    //    }
    //    _timer.Stop();

    //    return GetEmbed(cleancode, result, _timer.ElapsedMilliseconds);
    //}

    private static string GetUptime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
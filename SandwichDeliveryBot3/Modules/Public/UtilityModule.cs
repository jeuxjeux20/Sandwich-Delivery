﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Reflection;
using Microsoft.CodeAnalysis.Scripting;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot.ChefClass;
using SandwichDeliveryBot.SandwichClass;

namespace SandwichDeliveryBot.UtilityMod
{
    public class UtiltyModule : ModuleBase
    { 
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
                $"A user with `MANAGE_SERVER` can invite me to your server here: https://discordapp.com/oauth2/authorize?client_id=285522081775353856&scope=bot&permissions=3073");
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
                //$"- Current Order Count: {SS.activeOrders.Count}\n" +
                //$"- ToBeDelivered Count: {SS.toBeDelivered.Count}\n" +
                //$"- HasAnOrder Count: {SS.hasAnOrder.Count}\n" +
                $"- GivenFeedback Count: {SS.givenFeedback.Count}\n" +
                $"- ChefList Count: {SS.chefList.Count}\n" +
                $"- Blacklisted Count: {SS.blacklisted.Count}\n" +
                $"- Cache Count: {SS.cache.Count}\n" +
                //$"- Total Order Count: {SS.totalOrders}\n" +
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

        
    private static string GetUptime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
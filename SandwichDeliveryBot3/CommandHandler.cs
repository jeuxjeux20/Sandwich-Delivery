using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using System;
using Microsoft.Extensions.DependencyInjection;
using SandwichDeliveryBot.Databases;
using SandwichDeliveryBot3.CustomClasses;

namespace SandwichDeliveryBot.Handler
{
    public class CommandHandler
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider _provider;
        private UserDatabase _udb;
        private ArtistDatabase _adb;

        public async Task Install(IServiceProvider provider)
        {
            _provider = provider;
            client = _provider.GetService<DiscordSocketClient>();
            commands = _provider.GetService<CommandService>();
            _udb = _provider.GetService<UserDatabase>();
            _adb = _provider.GetService<ArtistDatabase>();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
            client.MessageReceived += HandleCommand;
        }

        public async Task HandleCommand(SocketMessage parameterMessage)
        {
            var message = parameterMessage as SocketUserMessage;
            if (message == null) return;
            if (message.Author.IsBot) return;
            int argPos = 0;
            if (!(message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.HasCharPrefix(';', ref argPos))) return;

            var context = new CommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, _provider);
            var a = await _udb.FindUser(message.Author.Id);
            if (a == null)
            {
                await _udb.CreateNewUser(message.Author);
            }
            if (!result.IsSuccess)
            {
                if (result.ErrorReason.ToLower() == "unknown command.")
                {
                    return;
                }
                else
                {
                    await message.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
                }
            }

        }
    }
}
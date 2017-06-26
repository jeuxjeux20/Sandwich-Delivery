using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace SandwichDeliveryBot.Handler
{
    public class CommandHandler
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider _provider;

        public async Task Install(IServiceProvider provider)
        {
            _provider = provider;

            client = _provider.GetService<DiscordSocketClient>();
            commands = _provider.GetService<CommandService>();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
            client.MessageReceived += HandleCommand;
        }

        public async Task HandleCommand(SocketMessage parameterMessage)
        {
            // Don't handle the command if it is a system message
            var message = parameterMessage as SocketUserMessage;
            if (message == null) return;
            if (message.Author.IsBot) return;
            // Mark where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message has a valid prefix, adjust argPos 
            if (!(message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.HasCharPrefix(';', ref argPos))) return;
            // Create a Command Context
            var context = new CommandContext(client, message);
            // Execute the Command, store the result
            var result = await commands.ExecuteAsync(context, argPos, _provider);
            // If the command failed, notify the user

            //if (!result.IsSuccess)
            //    await message.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
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

            // Console.WriteLine("beep boop made it through");
        }
    }
}

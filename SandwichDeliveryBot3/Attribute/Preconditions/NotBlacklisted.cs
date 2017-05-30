using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SandwichDeliveryBot.SService;
using System.Collections.Generic;

namespace NotBlacklistedPreCon
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class NotBlacklisted : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var user = context.User as SocketGuildUser;
            if (user == null)
                return Task.FromResult(PreconditionResult.FromError("The command was not used in a guild."));

            // due to an issue we're statically accessing the blacklist
            //SandwichService SandwichService = map.Get<SandwichService>();

            SandwichService SandwichService = map.Get<SandwichService>();

            Console.WriteLine(SandwichService.blacklisted.Count);
            var blacklisted = SandwichService.blacklisted;

            if(blacklisted.Contains(context.User.Id))
                return Task.FromResult(PreconditionResult.FromError("Your account is blacklisted from using this bot. Please note this is **not** a server blacklist."));

            if (blacklisted.Contains(context.Channel.Id))
                return Task.FromResult(PreconditionResult.FromError("This channel is blacklisted from using this bot. Please note this is **not** a server or a user blacklist."));

            if (blacklisted.Contains(context.Guild.Id))
                return Task.FromResult(PreconditionResult.FromError("This server is blacklisted from using this bot."));

            return Task.FromResult(PreconditionResult.FromSuccess());

        }
    }
}
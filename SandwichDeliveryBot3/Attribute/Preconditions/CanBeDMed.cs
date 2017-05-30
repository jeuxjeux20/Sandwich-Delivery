using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SandwichDeliveryBot.SService;

namespace CanBeDMEDPrecon
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class CanBeDMed : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var user = context.User as SocketGuildUser;
            if (user == null)
                return Task.FromResult(PreconditionResult.FromError("The command was not used in a guild."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}

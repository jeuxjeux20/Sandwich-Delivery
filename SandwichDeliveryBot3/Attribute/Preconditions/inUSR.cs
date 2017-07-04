using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SandwichDeliveryBot.SService;
using Microsoft.Extensions.DependencyInjection;

namespace SandwichDeliveryBot3.Precons
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class inUSR : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var user = context.User as SocketGuildUser;
            if (user == null)
                return Task.FromResult(PreconditionResult.FromError("The command was not used in a guild."));

            SandwichService SandwichService = provider.GetService<SandwichService>();

            if (context.Guild.Id == SandwichService.USRGuildId)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError("This command cannot be ran outside of our `;server`."));

        }
    }
}

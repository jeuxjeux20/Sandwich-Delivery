using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SandwichDeliveryBot.Databases;

namespace SandwichDeliveryBot3.Precons
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class NotBlacklisted : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var user = context.User as SocketGuildUser;
            if (user == null)
                return await Task.FromResult(PreconditionResult.FromError("The command was not used in a guild."));

            ListingDatabase listings = map.Get<ListingDatabase>();

            string r = await listings.CheckForBlacklist(context.Guild.Id);
            if (r != null)
            {
                return await Task.FromResult(PreconditionResult.FromError(r));
            }
            else
            {
                return await Task.FromResult(PreconditionResult.FromSuccess());
            }
        }
    }
}
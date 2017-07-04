using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SandwichDeliveryBot.Databases;
using Microsoft.Extensions.DependencyInjection;

namespace SandwichDeliveryBot3.Precons
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class NotBlacklisted : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var user = context.User as SocketGuildUser;
            if (user == null)
                return await Task.FromResult(PreconditionResult.FromError("The command was not used in a guild."));
            ListingDatabase listings;

            listings = provider.GetService<ListingDatabase>();


            string r = await listings.CheckForBlacklist(context.Guild.Id);
            string r2 = await listings.CheckForBlacklist(context.User.Id);
            if (r != null)
            {
                return await Task.FromResult(PreconditionResult.FromError(r));
            }
            if (r2 != null)
            {
                return await Task.FromResult(PreconditionResult.FromError(r2));
            }
            return await Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
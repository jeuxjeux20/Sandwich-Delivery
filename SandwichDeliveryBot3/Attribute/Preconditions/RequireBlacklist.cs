using System;
using System.Threading.Tasks;
using Discord.Commands;
using System.Linq;
using Discord.WebSocket;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot.Databases;
using SandwichDeliveryBot3.CustomClasses;

namespace SandwichDeliveryBot3.Precons
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class RequireBlacklist : PreconditionAttribute
    {

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {

            SandwichService SandwichService = map.Get<SandwichService>();
            ArtistDatabase Artistdatabase = map.Get<ArtistDatabase>();


            var user = context.User as SocketGuildUser;

            if (user == null)
                return Task.FromResult(PreconditionResult.FromError("The command was not used in a guild."));

            string[] roleNames = { "god sandwich artist", "admin", "senate" };

            var matchingRoles = context.Guild.Roles.Where(role => roleNames.Any(name => name == role.Name.ToLower()));

            if (matchingRoles == null)
                return Task.FromResult(PreconditionResult.FromError("There are no matching roles on the server."));

            if (user.Roles.Any(role => matchingRoles.Contains(role)))
            {
                Artist a = Artistdatabase.Artists.FirstOrDefault(x => x.ArtistId == context.User.Id);
                if (a != null)
                {
                    if (a.canBlacklist)
                        return Task.FromResult(PreconditionResult.FromSuccess());
                    else
                        return Task.FromResult(PreconditionResult.FromError("You do not have the ability to blacklist, which is required for this command to run."));


                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromError("You are not registered as a Sandwich Artist!"));
                }
            }

            return Task.FromResult(PreconditionResult.FromError("You do not have either the Senate, Admin or God Sandwich Artist role.."));


        }
    }
}

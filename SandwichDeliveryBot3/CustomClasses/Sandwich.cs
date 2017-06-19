using System;
using Newtonsoft.Json;
using SandwichDeliveryBot.OrderStatusEnum;
using Discord.Commands;

namespace SandwichDeliveryBot.SandwichClass
{
    [JsonObject]
    public class Sandwich
    {
        public string Id { get; set; }
        public string Desc { get; set; } //your order
        public DateTime date { get; set; }
        public OrderStatus Status { get; set; }
        public string AvatarUrl { get; set; }
        public string Discriminator { get; set; }
        public string UserName { get; set; }
        public string GuildIcon { get; set; }
        public string GuildName { get; set; }
        public ulong GuildDefaultChannelId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public ulong ArtistId { get; set; }

        //Moved from the AWFUL system of inputting all variables. Just give it context for fucks sake bro.
        public Sandwich(string d, string id, DateTime dat, OrderStatus s, ICommandContext context)
        {
            this.Desc = d; //setting the variables at the top to what we give the sandwich originally
            this.Id = id;
            this.date = dat;
            this.Status = s;
            this.AvatarUrl = context.User.GetAvatarUrl();
            this.Discriminator = context.User.Discriminator;
            this.UserName = context.User.Username;
            this.GuildIcon = context.Guild.IconUrl;
            this.GuildName = context.Guild.Name;
            this.GuildDefaultChannelId = context.Guild.DefaultChannelId;
            this.ChannelId = context.Channel.Id;
            this.UserId = context.User.Id;
            this.GuildId = context.Guild.Id;
        }

        public Sandwich()
        { }
    }
}
using Discord;
using Newtonsoft.Json;
using SandwichDeliveryBot.ArtistStatusEnum;
using System;

namespace SandwichDeliveryBot3.CustomClasses
{
    [JsonObject]
    public class Artist
    {
        public ulong ArtistId { get; set; } //Id of user who ordered
        public string ArtistName { get; set; } //your oder
        public string ArtistDistin { get; set; } //self explanatory
        public int ordersAccepted { get; set; } //self explanatory
        public int ordersDenied { get; set; } //self explanatory
        public ArtistStatus status { get; set; } = ArtistStatus.Trainee;
        public bool canBlacklist { get; set; } = false;
        public string HiredDate { get; set; }
        public int tipsRecieved { get; set; }
        public float Rating { get; set; }
        public int Ratings { get; set; }
        public DateTime lastOrder { get; set; }

        public Artist(IGuildUser newartist, string date) {
            this.ArtistId = newartist.Id;
            this.ArtistName = newartist.Username;
            this.ArtistDistin = newartist.Discriminator;
            this.HiredDate = date;
            this.ordersAccepted = 0;
            this.ordersDenied = 0;
            this.Rating = 2.5f;
            this.Ratings = 1;
            this.lastOrder = DateTime.Now;
        }

        public Artist() { }
    }
}

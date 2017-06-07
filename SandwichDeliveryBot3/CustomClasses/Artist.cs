using Newtonsoft.Json;
using SandwichDeliveryBot.ChefStatusEnum;

namespace SandwichDeliveryBot.ArtistClass
{
    [JsonObject]
    public class Artist
    {
        public ulong ArtistId { get; set; } //Id of user who ordered
        public string ArtistName { get; set; } //your oder
        public string ArtistDistin { get; set; } //self explanatory
        public int ordersAccepted { get; set; } //self explanatory
        public int ordersDelivered { get; set; } //self explanatory
        public ArtistStatus status { get; set; } = ArtistStatus.Trainee;
        public bool canBlacklist { get; set; } = false;
        public string HiredDate { get; set; }
    }
}

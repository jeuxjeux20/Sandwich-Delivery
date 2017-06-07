using Newtonsoft.Json;
using SandwichDeliveryBot.ChefStatusEnum;

namespace SandwichDeliveryBot.ChefClass
{
    [JsonObject]
    public class Chef
    {
        public ulong ChefId { get; set; } //Id of user who ordered
        public string ChefName { get; set; } //your oder
        public string ChefDistin { get; set; } //self explanatory
        public int ordersAccepted { get; set; } //self explanatory
        public int ordersDelivered { get; set; } //self explanatory
        public ArtistStatus status { get; set; } = ArtistStatus.Trainee;
        public bool canBlacklist { get; set; } = false;
        public string HiredDate { get; set; }
    }
}

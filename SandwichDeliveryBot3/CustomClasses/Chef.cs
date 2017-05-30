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
        public ChefStatus status { get; set; } = ChefStatus.Trainee;
        public bool canBlacklist { get; set; } = false;
        public string HiredDate { get; set; }

        public Chef(string d, ulong od, string sid, int cid, int sname, string oa, ChefStatus s)
        {
            this.ChefName = d;
            this.ChefId = od;
            this.ChefDistin = sid;
            this.ordersAccepted = cid;
            this.ordersDelivered = sname;
            this.HiredDate = oa;
            this.status = s;
        }

        
    }
}

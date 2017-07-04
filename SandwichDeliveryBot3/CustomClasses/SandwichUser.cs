using Discord;
using Discord.WebSocket;
using SandwichDeliveryBot3.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandwichDeliveryBot3.CustomClasses
{
    public class SandwichUser
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Distin { get; set; }
        public bool IsBlacklisted { get; set; }
        public int Orders { get; set; }
        public int Denials { get; set; }
        public int Tips { get; set; }
        public UserRank Rank { get; set; }
        public UserType Type { get; set; }

        public SandwichUser(SocketUser u)
        {
            Id = u.Id;
            Name = u.Username;
            Distin = u.Discriminator;
            IsBlacklisted = false;
            Orders = 1;
            Denials = 0;
            Tips = 3;
            Rank = UserRank.NewCustomer;
            Type = UserType.Customer;
        }
        public SandwichUser(IGuildUser u)
        {
            Id = u.Id;
            Name = u.Username;
            Distin = u.Discriminator;
            IsBlacklisted = false;
            Orders = 1;
            Denials = 0;
            Tips = 3;
            Rank = UserRank.NewCustomer;
            Type = UserType.Customer;
        }

        public SandwichUser() { }
    }
}

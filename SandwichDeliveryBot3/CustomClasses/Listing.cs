using Discord.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandwichDeliveryBot3.CustomClasses
{
    public enum ListingType {
        User,
        Guild
    }

    public class Listing
    {
        public ulong ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public ListingType Type { get; set; }
        public string Reason { get; set; }
        [Key]
        public int Case { get; set; }

        public Listing(string r, ListingType t, ulong id, string name)
        {
            Reason = r;
            Type = t;
            ID = id;
            Name = name;
            Date = DateTime.Now;
        }
    }
}

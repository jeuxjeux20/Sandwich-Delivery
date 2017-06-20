using System;
using System.ComponentModel.DataAnnotations;

namespace SandwichDeliveryBot3.CustomClasses
{
    public enum ListingType {
        User,
        Guild,
        Undefined
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

        public Listing(string r, ulong id, string name,ListingType t = ListingType.Undefined)
        {
            Reason = r;
            Type = t;
            ID = id;
            Name = name;
            Date = DateTime.Now;
        }
        public Listing()
        { }
    }
}

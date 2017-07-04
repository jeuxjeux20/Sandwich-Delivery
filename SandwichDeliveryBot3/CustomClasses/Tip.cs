using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandwichDeliveryBot3.CustomClasses
{
    public class Tip
    {
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public DateTime Date { get; set; }
        [Key]
        public int Id { get; set; }
        public Tip(string sender, string reciever)
        {
            this.Sender = sender;
            this.Reciever = reciever;
            Date = DateTime.Now;
        }
        public Tip() { }
    }
}

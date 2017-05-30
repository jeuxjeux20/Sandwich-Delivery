using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SandwichDeliveryBot.WarningClass
{
    public class Warning
    {
        [Required]
        public string UserName { get; set; }
        public ulong GuildId { get; set; }
        [Required]
        public ulong UserID { get; set; }
        public DateTime date { get; set; }
        public string Reason { get; set; }
        [Key]
        public int Case { get; set; }
    }
}

using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandwichDeliveryBot3
{
    public class Conversation
    {
        public string ConversationId { get; set; }

        public IEnumerable<IGuild> GuildsInvolved { get; set; }

        public IEnumerable<IUser> UsersInvolved { get; set; }

        public ConversationType Type { get; set; }

        public DateTime Date { get; set; }
    }
}
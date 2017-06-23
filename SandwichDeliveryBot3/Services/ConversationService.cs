using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandwichDeliveryBot3.Services
{
    class ConversationService
    {
        Dictionary<string, Conversation> conversations = new Dictionary<string, Conversation>();

        public string VerifyIdUniqueness(string id)
        {
            var s = conversations.FirstOrDefault(x => x.Value.ConversationId.ToLower() == id.ToLower());
            string newid;
            var r = new Random();
            if (s.Value != null)
            {
                newid = GenerateId(4);
                return newid;
            }
            else
            {
                return id;
            }
        }

        public string GenerateId(int Size)
        {
            string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var chars = Enumerable.Range(0, Size)
                                   .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }


    }
}

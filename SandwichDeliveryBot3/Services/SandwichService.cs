using System;
using Newtonsoft.Json;
using System.IO;

namespace SandwichDeliveryBot.SService
{
    public class SandwichService
    {
        public int totalOrders = 0;
        public string version = "3.1.2";
        public string date = "July 4th, 2017";
        public string updatename = "New user profile system.";
        public string motd;
        public ulong USRGuildId = 264222431172886529;
        public ulong KitchenId = 285529162511286282;
        public ulong LogId = 287990510428225537;
        public ulong TipId = 331646743039180801;



        public void Save()
        {
            try
            {

                using (var sw = new StreamWriter(@"data/ordercount.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, totalOrders);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }


        public void Load()
        {
            try
            {
                using (var sr = new StreamReader(@"data/ordercount.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    totalOrders = JsonSerializer.Create().Deserialize<int>(myLovelyReader);
                    Console.WriteLine(totalOrders);
                }
                using (var sr = new StreamReader(@"data/motd.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    motd = JsonSerializer.Create().Deserialize<string>(myLovelyReader);
                    Console.WriteLine(motd);
                }
            }
            catch
            {
                Console.WriteLine("Failed to save.");
            }
        }
    }
}
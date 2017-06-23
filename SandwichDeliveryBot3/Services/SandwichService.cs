using System;
using Newtonsoft.Json;
using System.IO;

namespace SandwichDeliveryBot.SService
{
    public class SandwichService
    {
        public int totalOrders = 0;
        public string version = "3.0.1";
        public string date = "June 23rd 2017";
        public string updatename = "Fixed minor 3.0 bugs.";
        public string motd;
        public ulong USRGuildId = 264222431172886529;
        public ulong KitchenId = 285529162511286282;
        public ulong LogId = 287990510428225537;



        public void Save()
        {
            try
            {

                using (var sw = new StreamWriter(@"data/ordercount.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, totalOrders);
                    Console.WriteLine("serialized order count");
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
                    Console.WriteLine("Deserialized number of total orders.");
                    Console.WriteLine(totalOrders);
                }
                using (var sr = new StreamReader(@"data/motd.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    motd = JsonSerializer.Create().Deserialize<string>(myLovelyReader);
                    Console.WriteLine("Deserialized motd.");
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
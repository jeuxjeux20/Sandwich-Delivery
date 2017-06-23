using System;
using Newtonsoft.Json;
using System.IO;

namespace SandwichDeliveryBot.SService
{
    public class SandwichService
    {
        public int totalOrders = 0;
        public string version = "3.0";
        public string date = "June 2017";
        public string updatename = "3.0";
        public string motd;
        public ulong USRGuildId = 264222431172886529;
        public ulong KitchenId = 264222431172886529;
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
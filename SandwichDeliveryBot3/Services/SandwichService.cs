using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Discord;
using Discord.Commands;
using SandwichDeliveryBot.SandwichClass;

namespace SandwichDeliveryBot.SService
{
    public class SandwichService
    {
        public int totalOrders = 0;
        public string version = "3.0";
        public string date = "June 2017";
        public string updatename = "3.0";
        public string motd;
        public ulong USRGuildId = 322455281286119466;
        public ulong KitchenId = 322455717254529034;
        public ulong LogId = 322463971359588352;



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
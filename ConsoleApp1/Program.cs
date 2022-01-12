


using CoreRCON;
using CoreRCON.PacketFormats;
using CoreRCON.Parsers.Standard;
using System;
using MineStatLib;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Settings settings = new Settings(Directory.GetCurrentDirectory(), "config.json");
            //ServerService serverService = new ServerService(settings);
            //ServerHandler handler = new ServerHandler(settings.ServerConfigs.FirstOrDefault(p => p.ServerName == "Mohist 1.12.2"));
            //handler.StartServer();
            //Console.ReadLine();
            //var log = new LogReceiver(50000, new IPEndPoint(IPAddress.Parse("92.52.138.234"), 25565));

            MineStat ms = new MineStat("mrkot9pa.pp.ua", 25565);
            Console.WriteLine("Minecraft server status of {0} on port {1}:", ms.Address, ms.Port);
            if (ms.ServerUp)
            {
                Console.WriteLine("Server is online running version {0} with {1} out of {2} players.", ms.Version, ms.CurrentPlayers, ms.MaximumPlayers);
                Console.WriteLine("Message of the day: {0}", ms.Motd);
                Console.WriteLine("Latency: {0}ms", ms.Latency);
            }
            else
                Console.WriteLine("Server is offline!");
        }
    }



    public class ColorLineParams
    {
        public string ColorType { get; set; }
        public string Line { get; set; }
    }


}



//var rcon = new RCON(IPAddress.Parse("92.52.138.234"), 25575, "123456");
//var responce = await rcon.SendCommandAsync("?");


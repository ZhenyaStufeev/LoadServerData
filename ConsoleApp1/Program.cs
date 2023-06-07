


using CoreRCON;
using CoreRCON.PacketFormats;
using CoreRCON.Parsers.Standard;
using System;
using MineStatLib;
using System.Threading.Tasks;
using System.Threading;

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




            MineStat ms = new MineStat("play.kot9pa.com", 25565);
            //MineStat ms = new MineStat("146.59.185.73", 25565);
            Console.WriteLine("Сервер {0}:{1}", ms.Address, ms.Port);
            if (ms.ServerUp)
            {
                Console.WriteLine("Играет {0} из {1} игроков", ms.CurrentPlayers, ms.MaximumPlayers);
                Console.WriteLine("Версия сервера: {0}", ms.Version);
                Console.WriteLine("Описание сервера: {0}", ms.Motd);
                Console.WriteLine("Отклик: {0}ms", ms.Latency);

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


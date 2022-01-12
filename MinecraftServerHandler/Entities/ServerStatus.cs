using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerHandler.Entities
{
    public class ServerStatus
    {
        public bool IsWorking { get; set; }
        public int CurrentOnline { get; set; }
        public int MaxOnline { get; set; }
        public string ServerIp { get; set; }
        public string ServerVersion { get; set; }
        public string ServerMotd { get; set; }
        public string ServerName { get; set; }
    }
}

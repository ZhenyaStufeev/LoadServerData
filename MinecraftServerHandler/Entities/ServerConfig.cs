using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerHandler.Entities
{
    public class ServerConfig
    {
        public string JavaHandler { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
        public string ServerName { get; set; }
        public string RconIp { get; set; }
        public int RconPort { get; set; }
        public string RconPassword { get; set; }
        public string ServerCore { get; set; }
        public string PathServerCore { get; set; }
        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public string SchemDirectory { get; set; }


    }
}

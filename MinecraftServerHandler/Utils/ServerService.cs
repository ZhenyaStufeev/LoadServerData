using CoreRCON;
using Microsoft.AspNetCore.SignalR;
using MinecraftServerHandler.Entities;
using MineStatLib;
using SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerHandler.Utils
{
    public class ServerService
    {
        private static List<ServerHandler> serverHandlers = new();
        private ColorBuilder colorBuilder;
        private static bool firstLoading = true;
        //public static List<ServerHandler> ServerHandlers { get => serverHandlers; private set => serverHandlers = value; }
        public Settings _settings;
        //RCON rcon;
        public ServerService(Settings settings)
        {
            _settings = settings;
            InitServerHandlers();
            colorBuilder = new ColorBuilder(_settings.ColorTags);
        }

        public ServerInfoResult GetServerInfo(string serverName)
        {
            var serverConfig = _settings.ServerConfigs.FirstOrDefault(p => p.ServerName == serverName);
            return new ServerInfoResult()
            {
                ServerName = serverConfig.ServerName,
                ServerIp = serverConfig.ServerIp + ":" + serverConfig.ServerPort,
                ServerSchems = serverConfig.SchemDirectory
            };
        }
        public List<ServerStatus> GetServersInfo()
        {
            return serverHandlers.Select(p => GetServerStatus(p)).ToList();
        }
        public bool SubscribeToSignalRServer(string ServerName, bool IsEnabledColor = true)
        {
            ServerHandler current_server = serverHandlers.FirstOrDefault(p => p.ServerConfig.ServerName == ServerName);
            if (current_server.ServerProcess == null)
            {
                return false;
            }
            _ = Task.Run(async () =>
            {
                StreamReader sr = current_server.ServerProcess.StandardOutput;

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    line = line.ToString();
                    Encoding UTF8 = Encoding.UTF8;
                    Encoding cp866 = Encoding.GetEncoding("CP866");
                    line = UTF8.GetString(cp866.GetBytes(line));

                    if (IsEnabledColor == true)
                    {
                        line = colorBuilder.ColorFormat(line);
                    }

                    if (SignalRHost.SignalRContext != null)
                    {
                        await SignalRHost.SignalRContext.Clients.All.SendAsync(ServerName, line.ToString());
                    }
                    else
                    {
                        Console.WriteLine(line);
                    }
                }

            });

            return true;
        }
        private void InitServerHandlers()
        {
            if (firstLoading == true)
            {
                serverHandlers.AddRange(_settings.ServerConfigs.Select(server_config => new ServerHandler(server_config)));
                firstLoading = false;
            }
        }
        public bool RunServer(string ServerName)
        {
            return serverHandlers.FirstOrDefault(sh => sh.ServerConfig.ServerName == ServerName).StartServer();
        }
        public bool SendCommand(CommandQuery query)
        {
            try
            {
                ServerHandler current_server = serverHandlers.FirstOrDefault(p => p.ServerConfig.ServerName == query.ServerName);
                if (current_server == null)
                    return false;
                StreamWriter input = current_server.ServerProcess.StandardInput;
                string command = query.Command;
                input.Write(command);
                input.Write("\n");
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool KillServerProcess(string ServerName)
        {
            ServerHandler current_server = serverHandlers.FirstOrDefault(p => p.ServerConfig.ServerName == ServerName);
            if (current_server == null)
            {
                return false;
            }
            current_server.StopServer();
            return true;
        }
        public ServerStatus GetServerStatus(string ServerName)
        {
            ServerHandler current_server = serverHandlers.FirstOrDefault(p => p.ServerConfig.ServerName == ServerName);
            return GetServerStatus(current_server);
        }
        private ServerStatus GetServerStatus(ServerHandler current_server)
        {
            if (current_server == null)
            {
                return null;
            }
            ServerStatus serverStatus = new ServerStatus();
            serverStatus.CurrentOnline = -1;
            serverStatus.IsWorking = current_server.ServerIsWorking;
            serverStatus.MaxOnline = -1;
            serverStatus.ServerIp = current_server.ServerConfig.ServerIp + ":" + current_server.ServerConfig.ServerPort;
            serverStatus.ServerVersion = "Не известно";
            serverStatus.ServerMotd = "Не известно";
            serverStatus.ServerName = current_server.ServerConfig.ServerName;

            if (current_server == null || current_server.ServerIsWorking == false)
            {
                return serverStatus;
            }
            MineStat ms = null;
            try
            {
                ms = new MineStat(current_server.ServerConfig.ServerIp, Convert.ToUInt16(current_server.ServerConfig.ServerPort));
            }
            catch
            {
                return serverStatus;
            }

            if (!ms.ServerUp)
            {
                return serverStatus;
            }

            serverStatus.CurrentOnline = ms.CurrentPlayersInt;
            serverStatus.IsWorking = current_server.ServerIsWorking;
            serverStatus.MaxOnline = ms.MaximumPlayersInt;
            serverStatus.ServerVersion = ms.Version;
            serverStatus.ServerMotd = ms.Motd;

            return serverStatus;
        }
    }
}

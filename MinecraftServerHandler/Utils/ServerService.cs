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
        private static List<ServerMonitor> serverMonitor = new();
        private ColorBuilder colorBuilder;
        private static bool firstLoading = true;
        public Settings _settings;
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
            return serverMonitor.Select(s_monitor => s_monitor.ServerStatus).ToList();
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
                _settings.ServerConfigs.ForEach(server_config =>
                {
                    ServerHandler handler = new ServerHandler(server_config);
                    serverHandlers.Add(handler);

                    ServerMonitor monitor = new ServerMonitor(handler);
                    serverMonitor.Add(monitor);
                });

                firstLoading = false;
            }
        }
        public bool RunServer(string ServerName)
        {
            var server_monitor = serverMonitor.FirstOrDefault(sm => sm.ServerHandler.ServerConfig.ServerName == ServerName);
            server_monitor.RunUpdateTimer(2000);
            var server_handler = server_monitor.ServerHandler;
            return server_handler.StartServer();
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
            var server_monitor = serverMonitor.FirstOrDefault(sm => sm.ServerHandler.ServerConfig.ServerName == ServerName);
            ServerHandler current_server = server_monitor.ServerHandler;
            if (current_server == null)
            {
                return false;
            }
            current_server.StopServer();
            return true;
        }
        public ServerStatus GetServerStatus(string ServerName)
        {
            //ServerHandler current_server = serverHandlers.FirstOrDefault(p => p.ServerConfig.ServerName == ServerName);
            var server_monitor = serverMonitor.FirstOrDefault(s_monitor
                                => s_monitor.ServerHandler.ServerConfig.ServerName
                                == ServerName);
            return server_monitor.ServerStatus;
        }
    }
}

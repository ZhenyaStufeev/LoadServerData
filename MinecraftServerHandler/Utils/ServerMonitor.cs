using CoreRCON.Parsers.Standard;
using MinecraftServerHandler.Entities;
using MineStatLib;
using System;
using System.Diagnostics;
using System.Timers;

namespace MinecraftServerHandler.Utils
{
    public class ServerMonitor
    {
        private System.Timers.Timer timer;
        private ServerHandler serverHandler;
        private ServerStatus serverStatus;
        public ServerStatus ServerStatus { get { return serverStatus; } }
        public ServerHandler ServerHandler { get { return serverHandler; } }
        public ServerMonitor(ServerHandler serverHandler)
        {
            this.serverHandler = serverHandler;
            serverStatus = InitServerStatus();
        }
        public void RunUpdateTimer(int timeout)
        {
            timer = new System.Timers.Timer(timeout);
            // Устанавливаем обработчик события Elapsed
            timer.Elapsed += UpdateTimerEvent;
            // Запускаем таймер
            timer.Start();
        }

        public void StopUpdateTimer()
        {
            timer.Stop();
            serverStatus = InitServerStatus();
        }

        private ServerStatus InitServerStatus()
        {
            ServerStatus serverStatus = new ServerStatus();
            serverStatus.CurrentOnline = -1;
            serverStatus.IsWorking = serverHandler.ServerIsWorking;
            serverStatus.MaxOnline = -1;
            serverStatus.ServerIp = serverHandler.ServerConfig.ServerIp + ":" + serverHandler.ServerConfig.ServerPort;
            serverStatus.ServerVersion = "Не известно";
            serverStatus.ServerMotd = "Не известно";
            serverStatus.ServerName = serverHandler.ServerConfig.ServerName;
            serverStatus.MemmoryUsage = 0;
            serverStatus.AllocatedMemmory = 0;

            return serverStatus;
        }

        private void UpdateTimerEvent(object sender, ElapsedEventArgs e)
        {

            if (serverHandler == null)
            {
                //return null;
                return;
            }

            if (serverHandler == null || serverHandler.ServerIsWorking == false)
            {
                //return serverStatus;
                StopUpdateTimer();
                return;
            }

            MineStat ms;
            try
            {
                ms = new MineStat(serverHandler.ServerConfig.ServerIp, Convert.ToUInt16(serverHandler.ServerConfig.ServerPort));
            }
            catch
            {
                //return serverStatus;
                return;
            }

            if (ms.ServerUp)
            {
                serverStatus.CurrentOnline = ms.CurrentPlayersInt;
                serverStatus.MaxOnline = ms.MaximumPlayersInt;
                serverStatus.ServerVersion = ms.Version;
                serverStatus.ServerMotd = ms.Motd;
            }
            else
            {
                serverStatus.CurrentOnline = -1;
                serverStatus.MaxOnline = -1;
                serverStatus.ServerVersion = "Не известно";
                serverStatus.ServerMotd = "Не известно";
            }

            Process current_server_procces = serverHandler.ServerProcess;
            if (current_server_procces != null && !current_server_procces.HasExited)
            {
                current_server_procces.Refresh();
                serverStatus.IsWorking = serverHandler.ServerIsWorking;
                serverStatus.AllocatedMemmory = serverHandler.ServerProcess.PrivateMemorySize64;
                serverStatus.MemmoryUsage = serverHandler.ServerProcess.WorkingSet64;
            }
            else
            {
                serverStatus.IsWorking = false;
                serverStatus.AllocatedMemmory = 0;
                serverStatus.MemmoryUsage = 0;
            }
        }
    }
}

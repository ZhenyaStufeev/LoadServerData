
using Microsoft.AspNetCore.SignalR;
using MinecraftServerHandler.Entities;
//using MinecraftServerHandler.Utils.JobManagement;
using SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerHandler.Utils
{
    public class ServerHandler
    {
        public ServerConfig ServerConfig { get; private set; }
        public Process ServerProcess { get; private set; }
        public Thread ServerThread { get; private set; }
        public bool ServerIsWorking { get; private set; }
        public ServerHandler(ServerConfig serverConfig)
        {
            ServerConfig = serverConfig;
            ServerIsWorking = false;
        }
        public bool StartServer()
        {

            if (ServerProcess != null)
            {
                return false;
            }
            ServerProcess = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ServerConfig.JavaHandler,
                    Arguments = ServerConfig.Arguments,
                    UseShellExecute = false,
                    WorkingDirectory = ServerConfig.WorkingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                },
                EnableRaisingEvents = true
            };
            ServerThread = new Thread(() =>
            {
                ServerProcess.Exited += new EventHandler(ExitEvent);
                bool IsStarted = ServerProcess.Start();

                if (IsStarted == true)
                {
                    ServerIsWorking = true;
                }
                else
                {
                    throw new Exception("An error occurred while creating a server process");
                }
                ChildProcessTracker.AddProcess(ServerProcess);
            })
            { IsBackground = true };
            ServerThread.Start();


            return true;
        }

        private void ExitEvent(object sender, EventArgs e)
        {
            StopServer();
        }

        public bool StopServer()
        {
            Console.WriteLine(ServerThread.IsAlive);
            if (ServerIsWorking == false)
            {
                return false;
            }
            ServerProcess.Kill();
            ServerProcess.Dispose();
            ServerProcess = null;
            ServerIsWorking = false;
            return true;
        }
    }
}

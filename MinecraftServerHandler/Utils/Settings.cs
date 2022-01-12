using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using MinecraftServerHandler.Entities;

namespace MinecraftServerHandler.Utils
{
    public class Settings
    {
        private ConfigurationBuilder builder { get; set; }
        private IConfigurationRoot config { get; set; }
        public List<ServerConfig> ServerConfigs { get; private set; }
        public List<ColorTag> ColorTags { get; private set; }
        public Settings(string basePath, string jsonFullFileName)
        {
            builder = new ConfigurationBuilder();
            
            builder.SetBasePath(basePath);
            builder.AddJsonFile(jsonFullFileName);
            config = builder.Build();
            this.InnitAppSettings();
        }
        private void InnitAppSettings()
        {
            ServerConfigs = GetServerConfigs();
            ColorTags = GetColorTagsConfig();
        }

        private List<ColorTag> GetColorTagsConfig()
        {
            List<ColorTag> configColors = new List<ColorTag>();
            var ConfigTags = config.GetSection("ColorTags").GetChildren().ToList();
            foreach (var Tag in ConfigTags)
            {
                ColorTag colorTag = new ColorTag()
                {
                    ColorType = Tag.GetSection("ColorType").Value,
                    StartTag = Tag.GetSection("StartTag").Value,
                    EndTag = Tag.GetSection("EndTag").Value
                };

                configColors.Add(colorTag);
            }
            return configColors;
        }

        private List<ServerConfig> GetServerConfigs()
        {
            List<ServerConfig> configs = new List<ServerConfig>();
            var ConfigServersInfo = config.GetSection("MinecraftServers").GetChildren().ToList();
            foreach (var ServerInfo in ConfigServersInfo)
            {
                ServerConfig ServerConfig = new ServerConfig()
                {
                    ServerName = ServerInfo.GetSection("ServerName").Value,
                    Arguments = ServerInfo.GetSection("Arguments").Value,
                    JavaHandler = ServerInfo.GetSection("JavaHandler").Value,
                    RconIp = ServerInfo.GetSection("RconIp").Value,
                    RconPort = int.Parse(ServerInfo.GetSection("RconPort").Value),
                    WorkingDirectory = ServerInfo.GetSection("WorkingDirectory").Value,
                    ServerCore = ServerInfo.GetSection("ServerCore").Value,
                    RconPassword = ServerInfo.GetSection("RconPassword").Value,
                    ServerIp = ServerInfo.GetSection("ServerIp").Value,
                    ServerPort = int.Parse(ServerInfo.GetSection("ServerPort").Value)
                };
                ServerConfig.PathServerCore = ServerConfig.WorkingDirectory + "\\" + ServerConfig.ServerCore;
                ServerConfig.Arguments = ServerConfig.Arguments.Replace("{path_server_core}", ServerConfig.PathServerCore);
                ServerConfig.SchemDirectory = ServerInfo.GetSection("SchemDirectory").Value.Replace("{working_directory}", (ServerConfig.WorkingDirectory));
                
                configs.Add(ServerConfig);
            }
            return configs;
        }
    }
}

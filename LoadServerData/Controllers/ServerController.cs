using LoadServerData.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinecraftServerHandler.Entities;
using MinecraftServerHandler.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadServerData.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class ServerController : ControllerBase
    {
        ServerService _serverService;
        public ServerController(ServerService serverService)
        {
            _serverService = serverService;
        }

        [HttpGet("getserversinfo")]
        public ActionResult<List<string>> GetServersInfo()
        {
            return Ok(_serverService.GetServersInfo());
        }

        [HttpGet("getserverinfo")]
        public ActionResult<List<string>> GetServerInfo([FromBody] ServerInfo serverinfo)
        {
            return Ok(_serverService.GetServerInfo(serverinfo.ServerName));
        }


        [HttpPost("runserver")]
        public ActionResult RunServerAsync([FromBody] ServerInfo serverinfo)
        {
            var res = _serverService.RunServer(serverinfo.ServerName);
            return Ok(res);
        }

        [HttpPost("subscribesignalrserver")]
        public ActionResult SubscribeSignalRServer([FromBody] ServerInfo serverinfo)
        {
            var res = _serverService.SubscribeToSignalRServer(serverinfo.ServerName);
            return Ok(res);
        }

        [HttpPost("sendcommand")]
        public ActionResult SendCommand([FromBody] CommandQuery commandQuery)
        {
            var res = _serverService.SendCommand(commandQuery);
            return Ok(res);
        }

        [HttpPost("killserver")]
        public ActionResult KillServer([FromBody] ServerInfo serverinfo)
        {
            var res = _serverService.KillServerProcess(serverinfo.ServerName);
            return Ok(res);
        }

        [HttpGet("getserverstatus/{servername}")]
        public ActionResult<List<string>> GetServerStatus(string servername)
        {
            return Ok(_serverService.GetServerStatus(servername));
        }
    }
}

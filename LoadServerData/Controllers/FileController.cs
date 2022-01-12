using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MinecraftServerHandler.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LoadServerData.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {

        ServerService _serverService;
        public FileController(ServerService serverService)
        {
            _serverService = serverService;
        }

        [HttpPost]
        public ActionResult Post([FromForm] FileModel file)
        {
            string path = _serverService._settings.ServerConfigs.FirstOrDefault(p => file.ServerName == p.ServerName).SchemDirectory;
            try
            {
                string path_gen = path + "/" + file.FileName;
                if (Path.GetExtension(path_gen) == ".schematic")
                {
                    using (Stream stream = new FileStream(path + "/" + file.FileName, FileMode.Create))
                    {
                        file.FormFile.CopyTo(stream);
                    }
                }
                else
                {

                }
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("getschematics/{servername}")]
        public ActionResult Get(string servername)
        {
            string path = _serverService._settings.ServerConfigs.FirstOrDefault(p => servername == p.ServerName).SchemDirectory;

            try
            {
                var fileNames = Directory.GetFiles(path);
                List<string> res_file_names = new();
                foreach (string fileName in fileNames)
                {
                    if (fileName.Contains(".schematic"))
                        res_file_names.Add(Path.GetFileName(fileName));
                }
                return Ok(res_file_names);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpDelete("{ServerName}/{FileName}")]
        public ActionResult Delete(string ServerName, string FileName)
        {
            string path = _serverService._settings.ServerConfigs.FirstOrDefault(p => ServerName == p.ServerName).SchemDirectory;

            try
            {
                string full_path = path + "\\" + FileName;
                if (System.IO.File.Exists(full_path))
                {
                    System.IO.File.Delete(full_path);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }
}

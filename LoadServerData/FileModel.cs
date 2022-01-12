using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadServerData
{
    public class FileModel
    {
        public string FileName { get; set; }
        public IFormFile FormFile { get; set; }
        public string ServerName { get; set; }
    }

    public class FileInfo
    { 
        public string FileName { get; set; }
    }

}

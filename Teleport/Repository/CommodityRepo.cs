using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Teleport.Models;

namespace Teleport.Repository
{
    public class CommodityRepo : ICommodityRepo
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string DirPath = "/Database/Commodity/";

        public CommodityRepo(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IEnumerable<Commodity> GetAll()
        {
            var dirPath = $"{_webHostEnvironment.ContentRootPath}{DirPath}";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var fileNames = Directory.EnumerateFiles(dirPath);
            return fileNames.Select(name => JsonConvert.DeserializeObject<Commodity>(name));
        }
    }
}
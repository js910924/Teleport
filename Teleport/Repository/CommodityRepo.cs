using System;
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
            EnsureDirectoryExist();

            var fileNames = Directory.EnumerateFiles(GetDirPath());
            return fileNames.Select(name => JsonConvert.DeserializeObject<Commodity>(File.ReadAllText(name)));
        }

        public void Add(Commodity commodity)
        {
            EnsureDirectoryExist();

            var filePath = $"{GetDirPath()}{commodity.Title}.json";
            if (!File.Exists(filePath))
            {
                commodity.Id = GetAll().Count() + 1;
                commodity.CreatedOn = DateTime.Now;
            }

            commodity.ModifiedOn = DateTime.Now;
            File.WriteAllText(filePath, JsonConvert.SerializeObject(commodity));
        }

        public void Remove(Commodity commodity)
        {
            EnsureDirectoryExist();

            var commodities = GetAll();
            var first = commodities.FirstOrDefault(commodityInDb => commodityInDb.Id == commodity.Id);
            if (first != null)
            {
                File.Delete($"{GetDirPath()}{first.Title}.json");
            }
        }

        private void EnsureDirectoryExist()
        {
            if (!Directory.Exists(GetDirPath()))
            {
                Directory.CreateDirectory(GetDirPath());
            }
        }

        private string GetDirPath()
        {
            return $"{_webHostEnvironment.ContentRootPath}{DirPath}";
        }
    }
}
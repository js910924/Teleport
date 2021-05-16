using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Teleport.Models;

namespace Teleport.Repository
{
    internal class StockInfoRepo : IStockInfoRepo
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string DirPath = "/Database/StockInfo/";

        public StockInfoRepo(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<StockInfo> GetStockInfo(string stockSymbol)
        {
            EnsureDirectoryExist();

            if (File.Exists($"{GetDirPath()}{stockSymbol}.json"))
            {
                var json = await File.ReadAllTextAsync($"{_webHostEnvironment.ContentRootPath}{DirPath}{stockSymbol}.json");

                return JsonConvert.DeserializeObject<StockInfo>(json);
            }

            return new StockInfo();
        }

        public async Task UpsertStockInfo(StockInfo stockInfo)
        {
            EnsureDirectoryExist();

            stockInfo.CreatedOn = DateTime.Now;

            var info = await GetStockInfo(stockInfo.Symbol);
            if (info.Symbol == stockInfo.Symbol)
            {
                stockInfo.CreatedOn = info.CreatedOn;
            }

            stockInfo.ModifiedOn = DateTime.Now;

            await File.WriteAllTextAsync($"{GetDirPath()}{stockInfo.Symbol}.json", JsonConvert.SerializeObject(stockInfo));
        }

        public async Task<IEnumerable<StockInfo>> GetAllStockInfo()
        {
            EnsureDirectoryExist();

            var filePaths = Directory.EnumerateFiles(GetDirPath());
            var tasks = filePaths.Select(filePath => File.ReadAllTextAsync(filePath));

            await Task.WhenAll(tasks);

            var stockInfos = Enumerable.Empty<StockInfo>().ToList();
            foreach (var task in tasks)
            {
                var json = await task;
                stockInfos.Add(JsonConvert.DeserializeObject<StockInfo>(json));
            }

            return stockInfos;
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
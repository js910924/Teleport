using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Teleport.Models;

namespace Teleport.Repository
{
    internal class StockInfoRepo : IStockInfoRepo
    {
        private const string FilePathPrefix = @"/app/Database/StockInfo/";

        public async Task<StockInfo> GetStockInfo(string stockSymbol)
        {
            if (File.Exists($"{FilePathPrefix}{stockSymbol}.json"))
            {
                var json = await File.ReadAllTextAsync($"{FilePathPrefix}{stockSymbol}.json");

                return JsonConvert.DeserializeObject<StockInfo>(json);
            }

            return new StockInfo();
        }

        public async Task UpsertStockInfo(StockInfo stockInfo)
        {
            stockInfo.CreatedOn = DateTime.Now;

            var info = await GetStockInfo(stockInfo.Symbol);
            if (info.Symbol == stockInfo.Symbol)
            {
                stockInfo.CreatedOn = info.CreatedOn;
            }

            stockInfo.ModifiedOn = DateTime.Now;

            await File.WriteAllTextAsync($"{FilePathPrefix}{stockInfo.Symbol}.json", JsonConvert.SerializeObject(stockInfo));
        }

        public async Task<IEnumerable<StockInfo>> GetAllStockInfo()
        {
            var filePaths = Directory.EnumerateFiles(FilePathPrefix);
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
    }
}
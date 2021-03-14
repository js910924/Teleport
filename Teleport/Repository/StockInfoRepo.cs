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
        private const string FilePath = @"/app/Database/StockInfo.json";

        public async Task<StockInfo> GetStockInfo(string stockSymbol)
        {
            var stockInfos = await GetAllStockInfo();

            var stockInfo = stockInfos.FirstOrDefault(info => info.Symbol == stockSymbol);

            return stockInfo ?? new StockInfo();
        }

        public async Task UpsertStockInfo(StockInfo stockInfo)
        {
            var stockInfos = (await GetAllStockInfo()).ToList();

            var info = stockInfos.FirstOrDefault(i => i.Symbol == stockInfo.Symbol);
            if (info != null)
            {
                stockInfo.CreatedOn = info.CreatedOn;
                stockInfos.Remove(info);
            }

            stockInfo.CreatedOn = DateTime.Now;
            stockInfo.ModifiedOn = DateTime.Now;
            stockInfos.Add(stockInfo);

            await File.WriteAllTextAsync(FilePath, JsonConvert.SerializeObject(stockInfos));
        }

        private static async Task<IEnumerable<StockInfo>> GetAllStockInfo()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = await File.ReadAllTextAsync(FilePath);

                    return JsonConvert.DeserializeObject<IEnumerable<StockInfo>>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Enumerable.Empty<StockInfo>();
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Teleport.Models;

namespace Teleport.Repository
{
    public class StockTransactionRepo : IStockTransactionRepo
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string DirPath = "/Database/Transaction/";

        public StockTransactionRepo(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task UpsertStockTransactions(IEnumerable<StockTransaction> stockTransactions, int customerId)
        {
            EnsureDirectoryExist();

            var json = JsonConvert.SerializeObject(stockTransactions);

            await File.WriteAllTextAsync($"{GetDirPath()}{customerId}_transactions.json", json);
        }

        public async Task InsertStockTransaction(StockTransaction stockTransaction)
        {
            EnsureDirectoryExist();

            var stockTransactions = (await GetStockTransactionsBy(stockTransaction.CustomerId)).ToList();

            stockTransaction.Id = !stockTransactions.Any() ? 1 : stockTransactions.Max(trx => trx.Id) + 1;
            stockTransactions.Add(stockTransaction);

            await UpsertStockTransactions(stockTransactions, stockTransaction.CustomerId);
        }

        public async Task DeleteTransaction(int transactionId, int customerId)
        {
            EnsureDirectoryExist();

            var stockTransactions = await GetStockTransactionsBy(customerId);

            var transactions = stockTransactions.Where(trx => trx.Id != transactionId);

            await UpsertStockTransactions(transactions, customerId);
        }

        public void DeleteAllTransactionsBy(int customerId)
        {
            EnsureDirectoryExist();

            File.Delete($"{GetDirPath()}{customerId}_transactions.json");
        }

        public async Task<IEnumerable<StockTransaction>> GetStockTransactionsBy(int customerId)
        {
            EnsureDirectoryExist();

            var filePath = $"{GetDirPath()}{customerId}_transactions.json";

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonConvert.DeserializeObject<IEnumerable<StockTransaction>>(json);
            }

            return Enumerable.Empty<StockTransaction>();
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
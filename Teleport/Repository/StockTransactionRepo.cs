using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Teleport.Models;

namespace Teleport.Repository
{
    public class StockTransactionRepo : IStockTransactionRepo
    {
        private const string DirectoryPath = @"./Database/Transaction/";

        public async Task UpsertStockTransactions(IEnumerable<StockTransaction> stockTransactions, int customerId)
        {
            var json = JsonConvert.SerializeObject(stockTransactions);

            await System.IO.File.WriteAllTextAsync($"{DirectoryPath}{customerId}_transactions.json", json);
        }

        public async Task InsertStockTransaction(StockTransaction stockTransaction)
        {
            var stockTransactions = (await GetStockTransactionsBy(stockTransaction.CustomerId)).ToList();

            stockTransaction.Id = !stockTransactions.Any() ? 1 : stockTransactions.Max(trx => trx.Id) + 1;
            stockTransactions.Add(stockTransaction);

            await UpsertStockTransactions(stockTransactions, stockTransaction.CustomerId);
        }

        public async Task DeleteTransaction(int transactionId, int customerId)
        {
            var stockTransactions = await GetStockTransactionsBy(customerId);

            var transactions = stockTransactions.Where(trx => trx.Id != transactionId);

            await UpsertStockTransactions(transactions, customerId);
        }

        public void DeleteAllTransactionsBy(int customerId)
        {
            System.IO.File.Delete($"{DirectoryPath}{customerId}_transactions.json");
        }

        public async Task<IEnumerable<StockTransaction>> GetStockTransactionsBy(int customerId)
        {
            var filePath = $"{DirectoryPath}{customerId}_transactions.json";

            if (System.IO.File.Exists(filePath))
            {
                var json = await System.IO.File.ReadAllTextAsync(filePath);
                return JsonConvert.DeserializeObject<IEnumerable<StockTransaction>>(json);
            }

            return Enumerable.Empty<StockTransaction>();
        }
    }
}
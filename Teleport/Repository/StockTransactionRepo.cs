using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Teleport.Models;

namespace Teleport.Repository
{
    public class StockTransactionRepo : IStockTransactionRepo
    {
        private const string FilePath = @"/app/Database/transactions.json";

        public async Task<IEnumerable<StockTransaction>> GetAllStockTransactions()
        {
            if (System.IO.File.Exists(FilePath))
            {
                var json = await System.IO.File.ReadAllTextAsync(FilePath);
                return JsonConvert.DeserializeObject<IEnumerable<StockTransaction>>(json);
            }

            return Enumerable.Empty<StockTransaction>();
        }

        public async Task UpsertStockTransactions(IEnumerable<StockTransaction> stockTransactions)
        {
            var json = JsonConvert.SerializeObject(stockTransactions);

            await System.IO.File.WriteAllTextAsync(FilePath, json);
        }

        public async Task UpsertStockTransaction(StockTransaction stockTransaction)
        {
            var stockTransactions = (await GetAllStockTransactions()).ToList();

            stockTransaction.Id = stockTransactions.Max(trx => trx.Id) + 1;
            stockTransactions.Add(stockTransaction);

            await UpsertStockTransactions(stockTransactions);
        }

        public async Task DeleteTransaction(int transactionId)
        {
            var stockTransactions = await GetAllStockTransactions();

            var transactions = stockTransactions.Where(trx => trx.Id != transactionId);

            await UpsertStockTransactions(transactions);
        }

        public async Task DeleteAllTransactionsBy(int customerId)
        {
            var stockTransactions = await GetAllStockTransactions();

            var transactions = stockTransactions.Where(trx => trx.CustomerId != customerId);

            await UpsertStockTransactions(transactions);
        }

        public async Task<IEnumerable<StockTransaction>> GetStockTransactionsBy(int customerId)
        {
            return (await GetAllStockTransactions()).Where(trx => trx.CustomerId == customerId);
        }
    }
}
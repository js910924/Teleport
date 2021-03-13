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

        public void DeleteAllHistoryTransactions()
        {
            System.IO.File.Delete(FilePath);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Repository
{
    public interface IStockTransactionRepo
    {
        Task<IEnumerable<StockTransaction>> GetAllStockTransactions();
        Task UpsertStockTransactions(IEnumerable<StockTransaction> stockTransactions);
        void DeleteAllHistoryTransactions();
        Task UpsertStockTransaction(StockTransaction stockTransaction);
    }
}
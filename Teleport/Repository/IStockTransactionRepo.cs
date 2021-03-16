using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Repository
{
    public interface IStockTransactionRepo
    {
        Task UpsertStockTransactions(IEnumerable<StockTransaction> stockTransactions, int customerId);
        Task InsertStockTransaction(StockTransaction stockTransaction);
        Task DeleteTransaction(int transactionId, int customerId);
        void DeleteAllTransactionsBy(int customerId);
        Task<IEnumerable<StockTransaction>> GetStockTransactionsBy(int customerId);
    }
}
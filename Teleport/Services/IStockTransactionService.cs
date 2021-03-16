using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Services
{
    public interface IStockTransactionService
    {
        Task UpsertStockTransaction(StockTransaction stockTransaction);
        Task DeleteTransaction(int id, int transactionId);
        Task<IEnumerable<StockTransaction>> GetAllStockTransactions();
        Task<IEnumerable<StockTransaction>> GetStockTransactionsBy(int customerId);
        Task DeleteAllTransactionsBy(int customerId);
    }
}
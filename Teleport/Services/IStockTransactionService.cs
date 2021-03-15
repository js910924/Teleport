using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Services
{
    public interface IStockTransactionService
    {
        Task UpsertStockTransaction(StockTransaction stockTransaction);
        Task DeleteTransaction(int transactionId);
        Task<IEnumerable<StockTransaction>> GetAllStockTransactions();
    }
}
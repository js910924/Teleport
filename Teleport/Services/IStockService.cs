using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Services
{
    public interface IStockService
    {
        Task<IEnumerable<StockPosition>> GetAllStockPositions();

        Task<List<StockTransaction>> UpsertStockTransaction(StockTransaction stockTransaction);

        Task<IEnumerable<StockTransaction>> GetAllStockTransactions();

        void DeleteAllTransactions();
        Task UpsertAllStockTransactions(IEnumerable<StockTransaction> stockTransactions);
    }
}
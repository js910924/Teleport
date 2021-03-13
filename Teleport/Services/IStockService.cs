using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Services
{
    public interface IStockService
    {
        Task<IEnumerable<StockPosition>> GetAllStockPositions();

        Task<List<StockTransaction>> UpsertStockTransactions(StockTransaction stockTransaction);

        Task<IEnumerable<StockTransaction>> GetAllStockTransactions();

        void DeleteAllTransactions();
    }
}
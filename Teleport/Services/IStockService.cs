using System.Collections.Generic;
using Teleport.Models;

namespace Teleport.Services
{
    public interface IStockService
    {
        IEnumerable<StockPosition> GetAllStockPositions(IEnumerable<StockTransaction> stockTransactions);
    }
}
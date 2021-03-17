using System.Threading.Tasks;
using System.Collections.Generic;
using Teleport.Models;

namespace Teleport.Repository
{
    public interface IStockInfoRepo
    {
        Task<StockInfo> GetStockInfo(string stockSymbol);
        Task UpsertStockInfo(StockInfo stockInfo);
        Task<IEnumerable<StockInfo>> GetAllStockInfo();
    }
}
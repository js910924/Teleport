using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Repository
{
    public interface IStockInfoRepo
    {
        Task<StockInfo> GetStockInfo(string stockSymbol);
        Task UpsertStockInfo(StockInfo stockInfo);
    }
}
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Proxy
{
    public interface IStockProxy
    {
        Task<StockInfo> GetStockInfo(string stockSymbol);
    }
}
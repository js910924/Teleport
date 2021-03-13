using Teleport.Models;

namespace Teleport.Proxy
{
    public interface IStockProxy
    {
        StockInfo GetStockInfo(string stockSymbol);
    }
}
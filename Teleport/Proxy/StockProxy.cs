using System;
using Teleport.Models;

namespace Teleport.Proxy
{
    public class StockProxy : IStockProxy
    {
        public StockInfo GetStockInfo(string stockSymbol)
        {
            if (stockSymbol == "AAPL")
            {
                return new StockInfo
                {
                    Symbol = stockSymbol,
                    Price = 200m,
                    PercentageOfChange = 0.0526m,
                    Change = 10m
                };
            }

            if (stockSymbol == "TSLA")
            {
                return new StockInfo
                {
                    Symbol = stockSymbol,
                    Price = 500m,
                    PercentageOfChange = -0.1667m,
                    Change = 100m
                };
            }

            throw new NotImplementedException();
        }
    }
}
using System;

namespace Teleport.Services
{
    public interface IStockMarketChecker
    {
        bool IsOpenMarket();
        bool IsTenSecondsAgo(DateTime dateTime);
        bool IsInOpenMarket(DateTime dateTime);
    }
}
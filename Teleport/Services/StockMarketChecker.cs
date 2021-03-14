using System;

namespace Teleport.Services
{
    public class StockMarketChecker : IStockMarketChecker
    {
        public bool IsOpenMarket()
        {
            var now = DateTime.Now.AddHours(-4);

            return IsInOpenMarket(now);
        }

        public bool IsTenSecondsAgo(DateTime dateTime)
        {
            return (DateTime.Now - dateTime).TotalSeconds >= 10;
        }

        public bool IsInOpenMarket(DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            var startTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 9, 30, 0);
            var closeTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 16, 00, 0);
            return dateTime >= startTime && dateTime <= closeTime;
        }
    }
}
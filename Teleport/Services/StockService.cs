using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teleport.Models;
using Teleport.Proxy;
using Teleport.Repository;

namespace Teleport.Services
{
    public class StockService : IStockService
    {
        private readonly IStockProxy _stockProxy;
        private readonly IStockTransactionRepo _stockTransactionRepo;
        private readonly IStockInfoRepo _stockInfoRepo;
        private readonly IStockMarketChecker _stockMarketChecker;
        private readonly IStockTransactionService _stockTransactionService;

        public StockService(IStockProxy stockProxy, IStockTransactionRepo stockTransactionRepo, IStockInfoRepo stockInfoRepo, IStockMarketChecker stockMarketChecker, IStockTransactionService stockTransactionService)
        {
            _stockProxy = stockProxy;
            _stockTransactionRepo = stockTransactionRepo;
            _stockInfoRepo = stockInfoRepo;
            _stockMarketChecker = stockMarketChecker;
            _stockTransactionService = stockTransactionService;
        }

        public async Task<IEnumerable<StockPosition>> GetAllStockPositions()
        {
            var stockTransactions = await _stockTransactionService.GetAllStockTransactions();

            var stockPositions = ToStockPositions(stockTransactions);

            // TODO: try to move this foreach to ToStockPositions method
            foreach (var stockPosition in stockPositions)
            {
                await GetRealTimeStockPosition(stockPosition);
            }

            return stockPositions;
        }

        public void DeleteAllTransactions()
        {
            _stockTransactionRepo.DeleteAllHistoryTransactions();
        }

        private static List<StockPosition> ToStockPositions(IEnumerable<StockTransaction> stockTransactions)
        {
            return stockTransactions
                .Where(trx => trx.Action == StockAction.Buy || trx.Action == StockAction.Sell)
                .GroupBy(trx => trx.Ticker)
                .Where(tickerGroup => tickerGroup.Sum(g => g.Action == StockAction.Buy ? g.Quantity : g.Quantity * -1) > 0)
                .Select(group =>
                {
                    var transactions = group.ToList();
                    var buyTransactions = transactions.Where(trx => trx.Action == StockAction.Buy).ToList();
                    var sellTransactions = transactions.Where(trx => trx.Action == StockAction.Sell).ToList();
                    var shares = (int) (buyTransactions.Sum(trx => trx.Quantity) - sellTransactions.Sum(trx => trx.Quantity));
                    var cost = buyTransactions.Sum(trx => trx.Quantity * trx.Price) - sellTransactions.Sum(trx => trx.Quantity * trx.Price);
                    return new StockPosition
                    {
                        Ticker = group.Key,
                        Shares = shares,
                        AveragePrice = Math.Round(cost / shares, 2),
                        Cost = cost,
                    };
                }).ToList();
        }

        private async Task GetRealTimeStockPosition(StockPosition position)
        {
            var stockInfo = await _stockInfoRepo.GetStockInfo(position.Ticker);
            if (stockInfo.Symbol != position.Ticker || ShouldGetStockInfoFromProxy(stockInfo))
            {
                stockInfo = await _stockProxy.GetStockInfo(position.Ticker);
                await _stockInfoRepo.UpsertStockInfo(stockInfo);
            }

            var currentValue = stockInfo.Price * position.Shares;
            var gain = currentValue - position.Cost;

            position.CurrentPrice = stockInfo.Price;
            position.CurrentValue = currentValue;
            position.PercentageOfChange = Math.Round(stockInfo.PercentageOfChange, 4);
            position.Change = stockInfo.Change;
            position.Gain = gain;
            position.PercentageOfGain = Math.Round(gain / position.Cost, 4);
        }

        private bool ShouldGetStockInfoFromProxy(StockInfo stockInfo)
        {
            var isOpenMarket = _stockMarketChecker.IsOpenMarket();
            var isModifiedOnInOpenMarket = _stockMarketChecker.IsInOpenMarket(stockInfo.ModifiedOn);
            if (!isOpenMarket && !isModifiedOnInOpenMarket)
            {
                return false;
            }

            var isModifiedOnTenSecondsAgo = _stockMarketChecker.IsTenSecondsAgo(stockInfo.ModifiedOn);
            if (isModifiedOnTenSecondsAgo)
            {
                return true;
            }

            return !isOpenMarket;
        }
    }
}
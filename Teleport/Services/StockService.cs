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
        private readonly IStockInfoRepo _stockInfoRepo;
        private readonly IStockMarketChecker _stockMarketChecker;
        private readonly IStockTransactionService _stockTransactionService;

        public StockService(IStockProxy stockProxy, IStockInfoRepo stockInfoRepo, IStockMarketChecker stockMarketChecker, IStockTransactionService stockTransactionService)
        {
            _stockProxy = stockProxy;
            _stockInfoRepo = stockInfoRepo;
            _stockMarketChecker = stockMarketChecker;
            _stockTransactionService = stockTransactionService;
        }

        public async Task<IEnumerable<StockPosition>> GetStockPositionsBy(int customerId)
        {
            var stockTransactions = await _stockTransactionService.GetStockTransactionsBy(customerId);

            var stockPositions = ToStockPositions(stockTransactions);

            // TODO: try to move this foreach to ToStockPositions method
            var tasks = Enumerable.Empty<Task>().ToList();
            tasks.AddRange(stockPositions.Select(position => GetRealTimeStockPosition(position)));
            Task.WaitAll(tasks.ToArray());

            return stockPositions;
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
                    var buyCost = buyTransactions.Sum(trx => trx.Quantity * trx.Price);
                    var buyShares = buyTransactions.Sum(trx => trx.Quantity);

                    var averageBuyPrice = buyCost / buyShares;
                    var currentShares = (int) transactions.Sum(trx => trx.Action == StockAction.Buy ? trx.Quantity : trx.Quantity * -1);

                    var cost = averageBuyPrice * currentShares;
                    return new StockPosition
                    {
                        Ticker = group.Key,
                        Shares = currentShares,
                        AveragePrice = Math.Round(cost / currentShares, 2),
                        Cost = cost,
                    };
                }).ToList();
        }

        private async Task GetRealTimeStockPosition(StockPosition position)
        {
            try
            {
                var stockInfo = await _stockInfoRepo.GetStockInfo(position.Ticker);
                if (stockInfo.Symbol != position.Ticker)
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
            catch (Exception e)
            {
                throw new Exception($"GetRealTimeStockPosition Fail | stock symbol = {position.Ticker}, exception = {e}");
            }
        }
    }
}
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

        public StockService(IStockProxy stockProxy, IStockTransactionRepo stockTransactionRepo, IStockInfoRepo stockInfoRepo)
        {
            _stockProxy = stockProxy;
            _stockTransactionRepo = stockTransactionRepo;
            _stockInfoRepo = stockInfoRepo;
        }

        public async Task<IEnumerable<StockPosition>> GetAllStockPositions()
        {
            var stockTransactions = await GetAllStockTransactions();

            var stockPositions =
                stockTransactions
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
                            AveragePurchasePrice = Math.Round(cost / shares, 2),
                            Cost = cost,
                        };
                    }).ToList();

            foreach (var stockPosition in stockPositions)
            {
                await GetRealTimeStockPosition(stockPosition);
            }

            return stockPositions;
        }

        public async Task<List<StockTransaction>> UpsertStockTransaction(StockTransaction stockTransaction)
        {
            var stockTransactions = (await GetAllStockTransactions()).ToList();

            stockTransactions.Add(stockTransaction);

            await _stockTransactionRepo.UpsertStockTransactions(stockTransactions);

            return stockTransactions;
        }

        public async Task<IEnumerable<StockTransaction>> GetAllStockTransactions()
        {
            return await _stockTransactionRepo.GetAllStockTransactions();
        }

        public void DeleteAllTransactions()
        {
            _stockTransactionRepo.DeleteAllHistoryTransactions();
        }

        private async Task GetRealTimeStockPosition(StockPosition position)
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
    }
}
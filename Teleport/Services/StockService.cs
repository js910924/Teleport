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

        public StockService(IStockProxy stockProxy, IStockTransactionRepo stockTransactionRepo)
        {
            _stockProxy = stockProxy;
            _stockTransactionRepo = stockTransactionRepo;
        }

        public async Task<IEnumerable<StockPosition>> GetAllStockPositions()
        {
            var stockTransactions = await GetAllStockTransactions();

            var stockPositions = stockTransactions
                .Where(trx => trx.Action == StockAction.Buy || trx.Action == StockAction.Sell)
                .GroupBy(trx => trx.Ticker, trx => trx, (ticker, tickerTransaction) =>
                {
                    var buyTransactions = tickerTransaction.Where(trx => trx.Action == StockAction.Buy);
                    var sellTransactions = tickerTransaction.Where(trx => trx.Action == StockAction.Sell);
                    var shares = (int) buyTransactions.Sum(trx => trx.Quantity) - (int) sellTransactions.Sum(trx => trx.Quantity);
                    var cost = buyTransactions.Sum(trx => trx.Quantity * trx.Price) - sellTransactions.Sum(trx => trx.Quantity * trx.Price);
                    return new StockPosition
                    {
                        Ticker = ticker,
                        Shares = shares,
                        AveragePurchasePrice = Math.Round(cost / shares, 2),
                        Cost = cost,
                    };
                }).ToList();

            var tasks = Enumerable.Empty<Task>().ToList();
            tasks.AddRange(stockPositions.Select(position => GetRealTimeStockPosition(position)));
            Task.WaitAll(tasks.ToArray());

            return stockPositions;
        }

        public async Task<List<StockTransaction>> UpsertStockTransactions(StockTransaction stockTransaction)
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
            var stockInfo = await _stockProxy.GetStockInfo(position.Ticker);

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
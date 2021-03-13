using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Teleport.Models;
using Teleport.Proxy;
using Teleport.Repository;

namespace Teleport.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly IStockTransactionRepo _stockTransactionRepo;
        private readonly IStockProxy _stockProxy;

        public PortfolioController(IStockTransactionRepo stockTransactionRepo, IStockProxy stockProxy)
        {
            _stockTransactionRepo = stockTransactionRepo;
            _stockProxy = stockProxy;
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var stockTransactions = await _stockTransactionRepo.GetAllStockTransactions();

            var stockTransactionDtos = stockTransactions.Select(trx => trx.ToStockTransactionDto());

            return View("History", stockTransactionDtos);
        }

        [HttpPost]
        public async Task<IActionResult> History(StockTransactionDto stockTransactionDto)
        {
            var stockTransaction = stockTransactionDto.ToStockTransaction();
            var stockTransactions = (await _stockTransactionRepo.GetAllStockTransactions()).ToList();

            stockTransactions.Add(stockTransaction);

            await _stockTransactionRepo.UpsertStockTransactions(stockTransactions);

            var stockTransactionDtos = stockTransactions.Select(trx => trx.ToStockTransactionDto());

            return View("History", stockTransactionDtos);
        }

        [HttpGet]
        public void DeleteAllHistoryTransactions()
        {
            _stockTransactionRepo.DeleteAllHistoryTransactions();
        }

        public async Task<ViewResult> Position()
        {
            var stockTransactions = await _stockTransactionRepo.GetAllStockTransactions();

            var stockPositions = GetAllStockPositions(stockTransactions);

            return View("Position", stockPositions);
        }

        private IEnumerable<StockPosition> GetAllStockPositions(IEnumerable<StockTransaction> stockTransactions)
        {
            var stockPositions = stockTransactions
                .GroupBy(trx => trx.Ticker, trx => trx, (ticker, tickerTransaction) =>
                {
                    var transactions = tickerTransaction.ToList();
                    var shares = (int) transactions.Sum(trx => trx.Quantity);
                    var cost = transactions.Sum(trx => trx.Quantity * trx.Price);
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
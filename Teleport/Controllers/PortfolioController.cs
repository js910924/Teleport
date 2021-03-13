using System;
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

            var stockPositions = stockTransactions.GroupBy(trx => trx.Ticker, trx => trx, (ticker, transaction) =>
            {
                var shares = (int)transaction.Sum(trx => trx.Quantity);
                var cost = transaction.Sum(trx => trx.Quantity * trx.Price);
                var stockInfo = _stockProxy.GetStockInfo(ticker);
                var currentValue = stockInfo.Price * shares;
                var gain = currentValue - cost;
                return new StockPosition
                {
                    Ticker = ticker,
                    Shares = shares,
                    AveragePurchasePrice = Math.Round(cost / shares, 2),
                    CurrentPrice = stockInfo.Price,
                    PercentageOfChange = Math.Round(stockInfo.PercentageOfChange, 4),
                    Change = stockInfo.Change,
                    Cost = cost,
                    CurrentValue = currentValue,
                    PercentageOfGain = Math.Round(gain / cost, 4),
                    Gain = gain
                };
            });

            return View("Position", stockPositions);
        }
    }
}
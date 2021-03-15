using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teleport.Models;
using Teleport.Repository;
using Teleport.Services;

namespace Teleport.Controllers
{
    [Authorize]
    public class PortfolioController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IStockTransactionService _stockTransactionService;
        private readonly IStockTransactionRepo _stockTransactionRepo;

        public PortfolioController(IStockService stockService, IStockTransactionService stockTransactionService, IStockTransactionRepo stockTransactionRepo)
        {
            _stockService = stockService;
            _stockTransactionService = stockTransactionService;
            _stockTransactionRepo = stockTransactionRepo;
        }

        [HttpGet]
        public async Task<ViewResult> History(int customerId)
        {
            var stockTransactions = await _stockTransactionService.GetStockTransactionsBy(customerId);

            var stockTransactionDtos = stockTransactions.Select(trx => trx.ToStockTransactionDto());

            var stockActions = Enum.GetValues(typeof(StockAction))
                .Cast<StockAction>()
                .Select(action => action.ToString())
                .ToList();

            var viewModel = new StockHistoryViewModel()
            {
                TransactionDtos = stockTransactionDtos.ToList(),
                CustomerId = customerId,
                StockActionDropDownList = stockActions
            };

            return View("History", viewModel);
        }

        [HttpGet]
        public async Task<RedirectToActionResult> DeleteAllHistoryTransactions(int customerId)
        {
            await _stockTransactionService.DeleteAllTransactionsBy(customerId);

            return RedirectToAction("History", customerId);
        }

        [HttpGet]
        public async Task<ViewResult> Position()
        {
            var stockPositions = await _stockService.GetAllStockPositions();

            return View("Position", stockPositions);
        }

        [HttpGet]
        public async Task<RedirectToActionResult> ImportMyExcelTransactions()
        {
            var data = await System.IO.File.ReadAllLinesAsync(@"/app/Database/realTransactions.txt");
            var stockTransactionDtos = data.Select(d =>
            {
                var strings = d.Split();
                var dateStrings = strings[0].Split('/');
                return new StockTransactionDto
                {
                    Date = $"{dateStrings[2]}-{dateStrings[1]}-{dateStrings[0]}",
                    Ticker = strings[1],
                    Action = strings[2],
                    Quantity = Convert.ToDecimal(strings[3]),
                    Price = Convert.ToDecimal(strings[4])
                };
            });

            var id = 1;
            var stockTransactions = stockTransactionDtos.Select(dto =>
            {
                var transaction = dto.ToStockTransaction();
                transaction.Id = id++;

                return transaction;
            });

            await _stockTransactionRepo.UpsertStockTransactions(stockTransactions);

            return RedirectToAction("History");
        }
    }
}
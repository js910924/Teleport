using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Teleport.Models;
using Teleport.Services;

namespace Teleport.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IStockTransactionService _stockTransactionService;

        public PortfolioController(IStockService stockService, IStockTransactionService stockTransactionService)
        {
            _stockService = stockService;
            _stockTransactionService = stockTransactionService;
        }

        [HttpGet]
        public async Task<ViewResult> History()
        {
            var stockTransactions = await _stockTransactionService.GetAllStockTransactions();

            var stockTransactionDtos = stockTransactions.Select(trx => trx.ToStockTransactionDto());

            return View("History", stockTransactionDtos);
        }

        [HttpGet]
        public RedirectToActionResult DeleteAllHistoryTransactions()
        {
            _stockService.DeleteAllTransactions();

            return RedirectToAction("History");
        }

        [HttpGet]
        public async Task<ViewResult> Position()
        {
            var stockPositions = await _stockService.GetAllStockPositions();

            return View("Position", stockPositions);
        }

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

            await _stockService.UpsertAllStockTransactions(stockTransactions);

            return RedirectToAction("History");
        }
    }
}
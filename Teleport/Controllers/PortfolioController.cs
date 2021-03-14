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

        public PortfolioController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<ViewResult> History()
        {
            var stockTransactions = await _stockService.GetAllStockTransactions();

            var stockTransactionDtos = stockTransactions.Select(trx => trx.ToStockTransactionDto());

            return View("History", stockTransactionDtos);
        }

        [HttpPost]
        public async Task<ViewResult> History(StockTransactionDto stockTransactionDto)
        {
            var stockTransaction = stockTransactionDto.ToStockTransaction();
            var stockTransactions = await _stockService.UpsertStockTransactions(stockTransaction);

            var stockTransactionDtos = stockTransactions.Select(trx => trx.ToStockTransactionDto());

            return View("History", stockTransactionDtos);
        }

        [HttpGet]
        public void DeleteAllHistoryTransactions()
        {
            _stockService.DeleteAllTransactions();
        }

        [HttpGet]
        public async Task<ViewResult> Position()
        {
            var stockPositions = await _stockService.GetAllStockPositions();

            return View("Position", stockPositions);
        }

        public async Task<JsonResult> ImportMyExcelTransactions()
        {
            var data = await System.IO.File.ReadAllLinesAsync(@"/app/Database/test.txt");
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

            foreach (var stockTransaction in stockTransactionDtos.Select(dto => dto.ToStockTransaction()))
            {
                await _stockService.UpsertStockTransactions(stockTransaction);
            }

            return new JsonResult("Done");
        }
    }
}
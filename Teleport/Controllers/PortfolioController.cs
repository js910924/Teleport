using System.Linq;
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
        public async Task<IActionResult> History()
        {
            var stockTransactions = await _stockService.GetAllStockTransactions();

            var stockTransactionDtos = stockTransactions.Select(trx => trx.ToStockTransactionDto());

            return View("History", stockTransactionDtos);
        }

        [HttpPost]
        public async Task<IActionResult> History(StockTransactionDto stockTransactionDto)
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

        public async Task<ViewResult> Position()
        {
            var stockPositions = await _stockService.GetAllStockPositions();

            return View("Position", stockPositions);
        }
    }
}
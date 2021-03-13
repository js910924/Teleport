using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Teleport.Models;
using Teleport.Repository;
using Teleport.Services;

namespace Teleport.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly IStockTransactionRepo _stockTransactionRepo;
        private readonly IStockService _stockService;

        public PortfolioController(IStockTransactionRepo stockTransactionRepo, IStockService stockService)
        {
            _stockTransactionRepo = stockTransactionRepo;
            _stockService = stockService;
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
            var stockPositions = _stockService.GetAllStockPositions(await _stockTransactionRepo.GetAllStockTransactions());

            return View("Position", stockPositions);
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Teleport.Models;
using Teleport.Repository;

namespace Teleport.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly IStockTransactionRepo _stockTransactionRepo;

        public PortfolioController(IStockTransactionRepo stockTransactionRepo)
        {
            _stockTransactionRepo = stockTransactionRepo;
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
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Teleport.Models;
using Teleport.Services;

namespace Teleport.Controllers
{
    public class StockTransactionController : Controller
    {
        private readonly IStockTransactionService _stockTransactionService;

        public StockTransactionController(IStockTransactionService stockTransactionService)
        {
            _stockTransactionService = stockTransactionService;
        }

        [HttpPost]
        public async Task<RedirectToActionResult> AddStockTransaction(StockTransactionDto stockTransactionDto, int customerId)
        {
            var stockTransaction = stockTransactionDto.ToStockTransaction();
            stockTransaction.CustomerId = customerId;

            await _stockTransactionService.UpsertStockTransaction(stockTransaction);

            return RedirectToAction("History", "Portfolio", customerId);
        }

        [HttpGet]
        public async Task<RedirectToActionResult> DeleteStockTransaction(int transactionId)
        {
            await _stockTransactionService.DeleteTransaction(transactionId);

            return RedirectToAction("History", "Portfolio");
        }
    }
}
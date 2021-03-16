using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teleport.Extention;
using Teleport.Models;
using Teleport.Services;

namespace Teleport.Controllers
{
    [Authorize]
    public class StockTransactionController : Controller
    {
        private readonly IStockTransactionService _stockTransactionService;

        public StockTransactionController(IStockTransactionService stockTransactionService)
        {
            _stockTransactionService = stockTransactionService;
        }

        [HttpPost]
        public async Task<RedirectToActionResult> AddStockTransaction(StockTransactionDto stockTransactionDto)
        {
            var customerId = User.GetCustomerId();
            var stockTransaction = stockTransactionDto.ToStockTransaction();
            stockTransaction.CustomerId = customerId;

            await _stockTransactionService.UpsertStockTransaction(stockTransaction);

            return RedirectToAction("History", "Portfolio");
        }

        [HttpGet]
        public async Task<RedirectToActionResult> DeleteStockTransaction(int transactionId)
        {
            var customerId = User.GetCustomerId();
            await _stockTransactionService.DeleteTransaction(transactionId, customerId);

            return RedirectToAction("History", "Portfolio");
        }
    }
}
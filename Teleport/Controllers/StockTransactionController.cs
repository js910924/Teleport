using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Teleport.Models;
using Teleport.Services;

namespace Teleport.Controllers
{
    public class StockTransactionController : BaseAuthorizeController
    {
        private readonly IStockTransactionService _stockTransactionService;

        public StockTransactionController(IStockTransactionService stockTransactionService)
        {
            _stockTransactionService = stockTransactionService;
        }

        [HttpPost]
        public async Task<RedirectToActionResult> AddStockTransaction(StockTransactionDto stockTransactionDto)
        {
            var stockTransaction = stockTransactionDto.ToStockTransaction();
            stockTransaction.CustomerId = CustomerId;

            await _stockTransactionService.UpsertStockTransaction(stockTransaction);

            return RedirectToAction("History", "Portfolio");
        }

        [HttpGet]
        public async Task<RedirectToActionResult> DeleteStockTransaction(int transactionId)
        {
            await _stockTransactionService.DeleteTransaction(transactionId, CustomerId);

            return RedirectToAction("History", "Portfolio");
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Teleport.Models;
using Teleport.Services;

namespace Teleport.Controllers
{
    public class StockTransactionController : Controller
    {
        private readonly IStockService _stockService;

        public StockTransactionController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost]
        public async Task<RedirectToActionResult> AddStockTransaction(StockTransactionDto stockTransactionDto)
        {
            var stockTransaction = stockTransactionDto.ToStockTransaction();

            await _stockService.UpsertStockTransaction(stockTransaction);

            return RedirectToAction("History", "Portfolio");
        }

        [HttpGet]
        public async Task<RedirectToActionResult> DeleteStockTransaction(int transactionId)
        {
            await _stockService.DeleteStockTransaction(transactionId);

            return RedirectToAction("History", "Portfolio");
        }
    }
}
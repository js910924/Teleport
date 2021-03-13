using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public async Task<IActionResult> Index()
        {
            var stockTransactions = await _stockTransactionRepo.GetAllStockTransactions();

            var json = JsonConvert.SerializeObject(stockTransactions.Select(trx => trx.ToStockTransactionDto()));

            return View("Index", json);
        }

        [HttpPost]
        public async Task<IActionResult> Index(StockTransactionDto stockTransactionDto)
        {
            var stockTransaction = stockTransactionDto.ToStockTransaction();
            var stockTransactions = (await _stockTransactionRepo.GetAllStockTransactions()).ToList();

            stockTransactions.Add(stockTransaction);

            await _stockTransactionRepo.UpsertStockTransactions(stockTransactions);

            var json = JsonConvert.SerializeObject(stockTransactions.Select(trx => trx.ToStockTransactionDto()));

            return View("Index", json);
        }

        [HttpGet]
        public void DeleteAllHistoryTransactions()
        {
            _stockTransactionRepo.DeleteAllHistoryTransactions();
        }
    }
}
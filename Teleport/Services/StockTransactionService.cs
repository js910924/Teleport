using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Models;
using Teleport.Repository;

namespace Teleport.Services
{
    public class StockTransactionService : IStockTransactionService
    {
        private readonly IStockTransactionRepo _stockTransactionRepo;

        public StockTransactionService(IStockTransactionRepo stockTransactionRepo)
        {
            _stockTransactionRepo = stockTransactionRepo;
        }

        public async Task UpsertStockTransaction(StockTransaction stockTransaction)
        {
            await _stockTransactionRepo.UpsertStockTransaction(stockTransaction);
        }

        public async Task DeleteTransaction(int transactionId)
        {
            await _stockTransactionRepo.DeleteTransaction(transactionId);
        }

        public async Task<IEnumerable<StockTransaction>> GetAllStockTransactions()
        {
            return await _stockTransactionRepo.GetAllStockTransactions();
        }
    }
}
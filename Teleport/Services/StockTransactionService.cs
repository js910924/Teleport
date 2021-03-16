using System;
using System.Collections.Generic;
using System.Linq;
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
            await _stockTransactionRepo.InsertStockTransaction(stockTransaction);
        }

        public async Task DeleteTransaction(int transactionId, int customerId)
        {
            var customerTransactions = await _stockTransactionRepo.GetStockTransactionsBy(customerId);
            if (customerTransactions.Any(trx => trx.Id == transactionId))
            {
                await _stockTransactionRepo.DeleteTransaction(transactionId);
            }

            throw new Exception(
                $"Transaction is not exist in this customer transactions, CustomerId = {customerId}, StockTransactionId = {transactionId}");
        }

        public async Task<IEnumerable<StockTransaction>> GetStockTransactionsBy(int customerId)
        {
            return await _stockTransactionRepo.GetStockTransactionsBy(customerId);
        }

        public async Task DeleteAllTransactionsBy(int customerId)
        {
            await _stockTransactionRepo.DeleteAllTransactionsBy(customerId);
        }
    }
}
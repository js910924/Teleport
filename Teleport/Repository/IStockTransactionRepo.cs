﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Repository
{
    public interface IStockTransactionRepo
    {
        Task<IEnumerable<StockTransaction>> GetAllStockTransactions();
        Task UpsertStockTransactions(IEnumerable<StockTransaction> stockTransactions);
        Task InsertStockTransaction(StockTransaction stockTransaction);
        Task DeleteTransaction(int transactionId);
        Task DeleteAllTransactionsBy(int customerId);
        Task<IEnumerable<StockTransaction>> GetStockTransactionsBy(int customerId);
    }
}
﻿using System;

namespace Teleport.Models
{
    public class StockTransaction
    {
        public DateTime Date { get; set; }
        public string Ticker { get; set; }
        public string Action { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedOn { get; set; }

        public StockTransactionDto ToStockTransactionDto()
        {
            return new()
            {
                Date = Date.ToString("MM/dd/yyyy"),
                Ticker = Ticker,
                Action = Action,
                Quantity = Quantity,
                Price = Price,
            };
        }
    }
}
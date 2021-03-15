using System;

namespace Teleport.Models
{
    public class StockTransaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Ticker { get; set; }
        public StockAction Action { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedOn { get; set; }

        public StockTransactionDto ToStockTransactionDto()
        {
            return new()
            {
                Id = Id,
                Date = Date.ToString("MM/dd/yyyy"),
                Ticker = Ticker,
                Action = Action.ToString(),
                Quantity = Quantity,
                Price = Price,
            };
        }
    }
}
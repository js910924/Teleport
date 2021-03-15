using System;

namespace Teleport.Models
{
    public class StockTransactionDto
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Ticker { get; set; }
        public string Action { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;

        public StockTransaction ToStockTransaction()
        {
            return new()
            {
                Id = Id,
                Date = Convert.ToDateTime(Date),
                Ticker = Ticker.ToUpper(),
                Action = Enum.Parse<StockAction>(Action),
                Quantity = Quantity,
                Price = Price,
                Total = Total,
                CreatedOn = DateTime.Now
            };
        }
    }
}
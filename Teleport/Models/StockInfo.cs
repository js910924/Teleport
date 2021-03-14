using System;

namespace Teleport.Models
{
    public class StockInfo
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal PercentageOfChange { get; set; }
        public decimal Change { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
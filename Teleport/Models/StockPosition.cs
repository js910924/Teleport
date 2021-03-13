namespace Teleport.Models
{
    public class StockPosition
    {
        public string Ticker { get; set; }
        public int Shares { get; set; }
        public decimal AveragePurchasePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal PercentageOfChange { get; set; }
        public decimal Change { get; set; }
        public decimal Cost { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal PercentageOfGain { get; set; }
        public decimal Gain { get; set; }
    }
}
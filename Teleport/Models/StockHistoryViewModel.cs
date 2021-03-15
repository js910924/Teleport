using System.Collections.Generic;

namespace Teleport.Models
{
    public class StockHistoryViewModel
    {
        public List<StockTransactionDto> TransactionDtos { get; set; }
        public int CustomerId { get; set; }
        public List<string> StockActionDropDownList { get; set; }
    }
}
namespace Teleport.Models
{
    public class ShoppingCartCommodityOperationRequest
    {
        public int CommodityId { get; set; }
        public string CommodityTitle { get; set; }
        public int Quantity { get; set; }
    }
}